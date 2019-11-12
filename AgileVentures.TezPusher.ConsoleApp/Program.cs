using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using AgileVentures.TezPusher.Model.Configuration;
using AgileVentures.TezPusher.Model.PushEntities;
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
        private static ConsoleAppConfig _appConfig;
        private static bool _keepRunning = true;

        private static readonly string TezosMonitorUrl = $"{_appConfig.Tezos.NodeUrl}/monitor/heads/main";
        private static readonly string MessageUrl = $"{_appConfig.Azure.AzureFunctionUrl}/api/message?code={_appConfig.Azure.AzureFunctionKey}";


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

            _appConfig = configuration.GetSection("Settings").Get<ConsoleAppConfig>();

            if (string.IsNullOrEmpty(_appConfig.Tezos.NodeUrl))
            {
                throw new ArgumentException(
                    $"NodeRpcEndpoint configuration is empty. Please provide a valid URL in .config file.");
            }

            if (string.IsNullOrEmpty(_appConfig.Azure.AzureFunctionUrl))
            {
                throw new ArgumentException(
                    $"AzureFunctionUrl configuration is empty. Please provide a valid URL in .config file.");
            }

            if (string.IsNullOrEmpty(_appConfig.Azure.AzureFunctionKey))
            {
                throw new ArgumentException(
                    $"AzureFunctionKey configuration is empty. Please provide a valid key in .config file.");
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
            while ((line = sr.ReadLine()) != null)
            {
                try
                {
                    var head = JsonConvert.DeserializeObject<HeadModel>(line);

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
            return $"{_appConfig.Tezos.NodeUrl}{string.Format("/chains/main/blocks/{0}", hash)}";
        }
    }
}
