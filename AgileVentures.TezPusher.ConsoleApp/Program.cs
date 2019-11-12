using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using AgileVentures.TezPusher.Model.Configuration;
using AgileVentures.TezPusher.Model.RpcEntities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NLog;
using NLog.Config;

namespace AgileVentures.TezPusher.ConsoleApp
{
    class Program
    {
        private static readonly HttpClient Client = new HttpClient(new HttpClientHandler
        {
            AllowAutoRedirect = false,
            MaxAutomaticRedirections = 20
        });

        private static Logger _log;
        private static AzureConfig _azureConfig;
        private static TezosConfig _tezosConfig;
        private static bool _keepRunning = true;

        private static string TezosMonitorUrl => $"{_tezosConfig.NodeUrl}/monitor/heads/main";
        private static string MessageUrl => $"{_azureConfig.AzureFunctionUrl}/api/message?code={_azureConfig.AzureFunctionKey}";


        static async Task Main(string[] args)
        {
            LogManager.Configuration = new XmlLoggingConfiguration($"{AppContext.BaseDirectory}nlog.config");
            _log = LogManager.GetLogger("AgileVentures.TezPusher.ConsoleApp");
            try
            {
                _log.Info("Starting Tezos Pusher");
                _log.Info("Press CTRL+C to exit the application.");

                GetConfiguration();
                _log.Info($"Configuration Loaded");

                RegisterGracefulShutdown();
                while (_keepRunning)
                {
                    try
                    {
                        await MonitorChainHead();
                    }
                    catch (Exception e)
                    {
                        _log.Error(e);
                    }
                }
            }
            catch (Exception e)
            {
                _log.Error(e, "Error occured!");
                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit(Avoid segmentation fault on Linux)
                LogManager.Shutdown();
            }
        }

        private static void RegisterGracefulShutdown()
        {
            Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e)
            {
                _log.Info("Stopping application gracefully...");
                e.Cancel = true;
                _keepRunning = false;
            };

            //Docker env.
            System.Runtime.Loader.AssemblyLoadContext.Default.Unloading += context =>
            {
                _log.Info("Stopping application gracefully...");
                _keepRunning = false;
            };
        }

        private static void GetConfiguration()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            _azureConfig = configuration.GetSection("Azure").Get<AzureConfig>();
            _tezosConfig = configuration.GetSection("Tezos").Get<TezosConfig>();

            if (string.IsNullOrEmpty(_tezosConfig.NodeUrl))
            {
                throw new ArgumentException(
                    $"Tezos:NodeUrl configuration is empty. Please provide a valid URL in appsettings.json or ENV variables.");
            }

            if (string.IsNullOrEmpty(_azureConfig.AzureFunctionUrl))
            {
                throw new ArgumentException(
                    $"Azure:AzureFunctionUrl configuration is empty. Please provide a valid URL in appsettings.json or ENV variables.");
            }

            if (string.IsNullOrEmpty(_azureConfig.AzureFunctionKey))
            {
                throw new ArgumentException(
                    $"Azure:AzureFunctionKey configuration is empty. Please provide a valid key in appsettings.json or ENV variables.");
            }
        }

        private static async Task MonitorChainHead()
        {
            
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var request = new HttpRequestMessage(HttpMethod.Get, TezosMonitorUrl);
            var result = await Client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, CancellationToken.None);
            var stream = await result.Content.ReadAsStreamAsync();
            var sr = new StreamReader(stream);
            string line;
            
            _log.Info("Started Tezos Node Monitoring");
            while ((line = sr.ReadLine()) != null && _keepRunning)
            {
                try
                {
                    var head = JsonConvert.DeserializeObject<MonitorHeadModel>(line);

                    var blockString = await Client.GetStringAsync(GetBlockUrl(head.hash));

                    await Client.PostAsync(MessageUrl, new StringContent(blockString));

                    _log.Info($"Block {head.level} has been sent for processing.");
                    _log.Trace(line);
                }
                catch (Exception ex)
                {
                    _log.Error(ex, $"Failed to send the block for processing.");
                }
            }
        }

        private static string GetBlockUrl(string hash)
        {
            return $"{_tezosConfig.NodeUrl}{string.Format("/chains/main/blocks/{0}", hash)}";
        }
    }
}
