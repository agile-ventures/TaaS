using System;
using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using AgileVentures.TezPusher.Model;
using Newtonsoft.Json;
using NLog;
using NLog.Config;

namespace AgileVentures.Tezos.Pusher
{
    class Program
    {
        private static readonly HttpClient Client = new HttpClient(new HttpClientHandler
        {
            AllowAutoRedirect = false,
            MaxAutomaticRedirections = 20
        });

        private static Logger Log;

        static async Task Main(string[] args)
        {
            LogManager.Configuration = new XmlLoggingConfiguration($"{AppContext.BaseDirectory}nlog.config");
            Log = LogManager.GetLogger("AgileVentures.Tezos.Pusher");

            Log.Info("Starting Tezos Pusher");
            
            var nodeUrl = $"{ConfigurationManager.AppSettings.Get("TezosNodeRpcEndpoint")}/monitor/heads/main";
            var postUrl = $"{ConfigurationManager.AppSettings.Get("AzureFunctionEndpoint")}/api/message";
            do
            {
                while (!Console.KeyAvailable)
                {
                    try
                    {
                        await GetChainHead(nodeUrl, postUrl);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }
            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);


            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        private static async Task GetChainHead(string url, string postUrl)
        {
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            var result = await Client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, CancellationToken.None);
            var stream = await result.Content.ReadAsStreamAsync();
            var sr = new StreamReader(stream);
            string line;

            while ((line = sr.ReadLine()) != null)
            {
                try
                {
                    await Client.PostAsync(postUrl, new StringContent(line));
                    var head = JsonConvert.DeserializeObject<HeadModel>(line);
                    
                    Log.Info($"Block {head.level} sent to SignalR.");
                    Log.Trace(line);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, $"Failed to deserialize json object.");
                }

                
            }
        }
    }
}
