﻿using System;
using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using AgileVentures.TezPusher.Model.PushEntities;
using Newtonsoft.Json;
using NLog;
using NLog.Config;

namespace AgileVentures.TezPusher.Pusher.ConsoleApp
{
    class Program
    {
        private static readonly HttpClient Client = new HttpClient(new HttpClientHandler
        {
            AllowAutoRedirect = false,
            MaxAutomaticRedirections = 20
        });

        private static Logger Log;
        private static readonly string TezosNodeUrl = ConfigurationManager.AppSettings.Get("TezosNodeRpcEndpoint");
        private static readonly string AzureFunctionUrl = ConfigurationManager.AppSettings.Get("AzureFunctionEndpoint");

        private static readonly string TezosMonitorUrl = $"{TezosNodeUrl}/monitor/heads/main";
        private static readonly string MessageUrl = $"{AzureFunctionUrl}/api/message";
        private static readonly string TransactionUrl = $"{AzureFunctionUrl}/api/transactions";


        static async Task Main(string[] args)
        {
            LogManager.Configuration = new XmlLoggingConfiguration($"{AppContext.BaseDirectory}nlog.config");
            Log = LogManager.GetLogger("AgileVentures.Tezos.Pusher");
            try
            {
                Log.Info("Starting Tezos Pusher");

                if (string.IsNullOrEmpty(TezosNodeUrl))
                {
                    throw new ArgumentException($"NodeRpcEndpoint configuration is empty. Please provide a valid URL in either .config file or if you are running in Docker in ENV variable.");
                }
                if (string.IsNullOrEmpty(AzureFunctionUrl))
                {
                    throw new ArgumentException($"AzureFunctionUrl configuration is empty. Please provide a valid URL in either .config file or if you are running in Docker in ENV variable.");
                }

                do
                {
                    while (!Console.KeyAvailable)
                    {
                        try
                        {
                            await MonitorChainHead();
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
            catch (Exception e)
            {
                Log.Error(e, "Error occured!");
                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit(Avoid segmentation fault on Linux)
                LogManager.Shutdown();
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

            while ((line = sr.ReadLine()) != null)
            {
                try
                {
                    //await Client.PostAsync(postUrl, new StringContent(line));
                    var head = JsonConvert.DeserializeObject<HeadModel>(line);

                    var blockString = await Client.GetStringAsync(GetBlockUrl(head.hash));

                    await Client.PostAsync(MessageUrl, new StringContent(line));
                    await Client.PostAsync(TransactionUrl, new StringContent(blockString));

                    Log.Info($"Block {head.level} has been sent for processing.");
                    Log.Trace(line);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, $"Failed to send the block for processing.");
                }
            }
        }

        private static string GetBlockUrl(string hash)
        {
            return $"{TezosNodeUrl}{string.Format("/chains/main/blocks/{0}", hash)}";
        }
    }
}