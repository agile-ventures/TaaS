


# TaaS (Tezos as a Service)
TaaS provides real-time updates to various applications from the Tezos Blockchain by leveraging SignalR. 

## Table of contents
 - [How to use](#how-to-use)
 - [Solution description](#solution-description)
 - [Sample Client Applications](#sample-client-applications)

## How to use

### Option #1 - Running [Pusher.Web](https://github.com/agile-ventures/TaaS/tree/master/AgileVentures.TezPusher.Pusher.Web) in Docker (most convenient at the moment)
Ready-to-use docker image is available from Docker Hub here: 
[https://hub.docker.com/r/tezoslive/agileventurestezpusherweb](https://hub.docker.com/r/tezoslive/agileventurestezpusherweb).

#### Configuration needed
Provide a configuration for `Pusher.Web` project in 
- the `ENV` variable `Tezos:NodeUrl` has to be set. Configured Tezos RPC endpoint has to support monitor call (`monitor/heads/main`).

For client side instructions please see [Subscribing to events from the client - Option 1 or 2](#i-am-using-option-1-or-2).

### Option #2 - Running [Pusher.Web](https://github.com/agile-ventures/TaaS/tree/master/AgileVentures.TezPusher.Pusher.Web) as a standalone ASP.NET Core app
Configuration needed
Provide a configuration for `Pusher.Web` project in 
- `appsettings.json` file. You will need to fill in this value `"NodeUrl": ""` - Configured Tezos RPC endpoint has to support monitor call (`monitor/heads/main`).

For client side instructions please see [Subscribing to events from the client - Option 1 or 2](#i-am-using-option-1-or-2).

### Option #3 - Using Azure Functions and TezPusher.ConsoleApp
Configuration needed
Provide a configuration for `Pusher.ConsoleApp` project in the `app.config` file. You will need to fill in these values
 - `<add key="NodeRpcEndpoint" value="TODO" />` Tezos RPC endpoint has to support monitor calls
 - `<add key="AzureFunctionEndpoint" value="TODO" />` Endpoint of your deployed function app

Provide a configuration for `Function` project in the `local.settings.json` file. 
There is a pre-filled endpoint which is hosted on Azure Free plan, so it might be already above daily threshold. You can create a SignalR Service on Azure for free on [Azure](https://azure.microsoft.com/en-us/) and provide your own SignalR connection string.
 - `"AzureSignalRConnectionString": ""`	

For client side instructions please see [Subscribing to events from the client - Option 3 or 4](#i-am-using-option-3-or-4).

### Option #4 - Using the endpoint from [TezosLive.io](https://tezoslive.io)
Sign in using your GitHub account on [TezosLive.io](https://tezoslive.io) and request your endpoint. There is no need to setup or host anything on the server side. 

API is currently limited to 
- 20 000 messages per account per day (1 message is counted for each 64kB in case message has more than 64kB)
- 20 concurrent connection per account

For client side instructions please see [Subscribing to events from the client - Option 3 or 4](#i-am-using-option-3-or-4).

## Subscribing to events from the client

### I am using option #1 or #2

You can connect to the hub for example like this (see `signalr.service.ts`)
```typescript
private  connect():  Observable<any> {
	this.hubConnection  = new signalR.HubConnectionBuilder()
		.withUrl(`${this._baseUrl}/tezosHub`)
		.configureLogging(signalR.LogLevel.Information)
		.build();
	return  from(this.hubConnection.start());
}
```
You can then subscribe to transactions like this. 
```typescript
this.hubConnection.send("subscribe", { transactionAddresses: ['all'] });
```
Note: `transactionAddresses` is a `string[]`. Specifying `'all'` will subscribe the client to all transactions.

For reference please take a look at [AgileVentures.TezPusher.SampleClient.Web](https://github.com/agile-ventures/TaaS/tree/master/AgileVentures.TezPusher.SampleClient.Web) specifically [`signalr.service.ts`](https://github.com/agile-ventures/TaaS/blob/84fe386b38f5e488a194a2aa531b109c7dc435d6/AgileVentures.TezPusher.SampleClient.Web/src/app/signalr.service.ts#L65).

### I am using option #3 or #4

You will need to provide a [UUID](https://en.wikipedia.org/wiki/Universally_unique_identifier) in a custom HTTP header named `x-tezos-live-userid` to identify a client during the initial call to `negotiate` endpoint. In the sample client application we are using the [npm uuid package](https://www.npmjs.com/package/uuid) to generate random UUIDs. 

You can see how the subscription to all transactions is being made by looking at the `signalr.service.ts` [here](https://github.com/agile-ventures/TaaS/blob/master/AgileVentures.TezPusher.SampleClient/src/app/signalr.service.ts) by making a `POST` request to `subscribe` endpoint with the following parameters

 - userId `[string]` - this is the UUID you have used for the `negotiate` call
 - transactionAddresses`[string[]]` - this is the array of the addresses that you want to subscribe to. You can subscribe to all addresses by sending `['all']`.

You can also subscribe only to a subset of addresses, that you are interested in by providing them as a parameter to `subscribe` call. 
You need to provide the generated UUID that you used in the `negotiate` call along with the array of the addresses.

For reference please take a look at  [AgileVentures.TezPusher.SampleClient](https://github.com/agile-ventures/TaaS/tree/master/AgileVentures.TezPusher.SampleClient).

Or you can check out deployed version of this app available here [https://client-staging.tezoslive.io/](https://client-staging.tezoslive.io/).

## Solution Description
Solution consists of several projects described bellow

 - [AgileVentures.TezPusher.Function](https://github.com/agile-ventures/TaaS/tree/master/AgileVentures.TezPusher.Function)
Azure Function getting the updates from Pusher.ConsoleApp and sending the updates to SignalR hub.

 - [AgileVentures.TezPusher.Model](https://github.com/agile-ventures/TaaS/tree/master/AgileVentures.TezPusher.Model)
Simple Model for the updates. This will be extended heavily based on the different subscriptions.

 - [AgileVentures.TezPusher.Pusher.ConsoleApp](https://github.com/agile-ventures/TaaS/tree/master/AgileVentures.TezPusher.Pusher.ConsoleApp)
Small Console Application in .NET Core used to monitor Tezos Node and push updates to the Azure Function. 

 - [AgileVentures.TezPusher.Pusher.Web](https://github.com/agile-ventures/TaaS/tree/master/AgileVentures.TezPusher.Pusher.Web)
ASP.NET Core Application, that monitors Tezos Node and also provides updates to clients through SignalR hub over WebSocket transport. 

	**Docker supported!** 
	To try-out docker version you can also get it from  Docker Hub here [https://hub.docker.com/r/tezoslive/agileventurestezpusherweb](https://hub.docker.com/r/tezoslive/agileventurestezpusherweb).  See instructions for [Option #1](#option-1---running-pusherweb-in-docker-most-convenient-at-the-moment).

 - [AgileVentures.TezPusher.SampleClient](https://github.com/agile-ventures/TaaS/tree/master/AgileVentures.TezPusher.SampleClient)
 Sample Client application written in Angular consuming the updates provided by the Azure SignalR hub.   
 
 - [AgileVentures.TezPusher.SampleClient.Web](https://github.com/agile-ventures/TaaS/tree/master/AgileVentures.TezPusher.SampleClient.Web)
 Sample Client application written in Angular consuming the updates provided by the  ASP.NET Core SignalR hub. 

### Sample Client Applications
- For [Option #1](#option-1---running-pusherweb-in-docker-most-convenient-at-the-moment) & [Option #2](#option-2---running-pusherweb-as-a-standalone-aspnet-core-app) - TODO
- For  [Option #3](#option-3---using-azure-functions-and-tezpusherconsoleapp) & [Option #4](#option-4---using-the-endpoint-from-tezosliveio) at [https://client-staging.tezoslive.io/](https://client-staging.tezoslive.io/)
