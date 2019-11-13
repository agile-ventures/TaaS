# TaaS \(Tezos as a Service\)

### About TaaS

TaaS provides real-time updates to various applications based on the events happening on Tezos by leveraging SignalR \(WebSocket\).

### Documentation

[https://docs.tezoslive.io/docs-welcome](https://docs.tezoslive.io/docs-welcome)

## Table of contents

* [How to use](./#how-to-use)
* [Solution description](./#solution-description)
* [Sample Client Applications](./#sample-client-applications)

## How to use

### Option \#1 - Running [Pusher.Web](https://github.com/agile-ventures/TaaS/tree/master/AgileVentures.TezPusher.Pusher.Web) in Docker

Ready-to-use docker image is available from Docker Hub here: [https://hub.docker.com/r/tezoslive/agileventurestezpusherweb](https://hub.docker.com/r/tezoslive/agileventurestezpusherweb).

You can start the container by using the following command

```text
docker run --rm -it -p 80:80 \
--env Tezos:NodeUrl="http://172.17.0.1:8732" \
tezoslive/agileventurestezpusherweb
```

This will expose port `80` to the host and set your Tezos Node RPC to `http://172.17.0.1:8732`.

{% hint style="warning" %}
**Do not forget to replace the NodeUrl per your environment!**
{% endhint %}

Please make sure to check the [documentation](https://docs.tezoslive.io/docs-getting-started/docs-using-docker) for additional information.

#### Configuration needed

Provide a configuration for `Pusher.Web` project in

* the `ENV` variable `Tezos:NodeUrl` has to be set. Configured Tezos RPC endpoint ****must support following calls 
  * `monitor/heads/main` 
  * `/chains/main/blocks/{hash}`

For client side instructions please see [Subscribing to events from the client - Option 1 or 2](./#i-am-using-option-1-or-2).

### Option \#2 - Running [Pusher.Web](https://github.com/agile-ventures/TaaS/tree/master/AgileVentures.TezPusher.Pusher.Web) as a standalone ASP.NET Core app

Provide a configuration for `Pusher.Web` project in

* `appsettings.json` file. You will need to fill in this value `"NodeUrl": ""` . Configured Tezos RPC endpoint must support following calls 
  * `monitor/heads/main` 
  * `/chains/main/blocks/{hash}`

For client side instructions please see [Subscribing to events from the client - Option 1 or 2](./#i-am-using-option-1-or-2).

### Option \#3 - Using Azure Functions and TezPusher.ConsoleApp

#### ConsoleApp Configuration

Provide a configuration for `ConsoleApp` project in the `appsettings.json` file if you are running from compiled sources or `ENV` variables if you are running from Docker. 

{% hint style="warning" %}
Be sure to configure the following keys correctly per your environment

* `Tezos:NodeUrl` - Tezos RPC endpoint URL
* `Azure:AzureFunctionUrl` - URL of your deployed function app
* `Azure:AzureFunctionKey` - Access key for your message function of your deployed function app
{% endhint %}

#### Function App Configuration

Provide a configuration for `Function` project in the `local.settings.json` file if you are running it locally or Azure Applications Settings if you are running in Azure. There is a pre-filled endpoint which is hosted on Azure Free plan, so it might be already above daily threshold. You can create a SignalR Service on Azure for free on [Azure](https://azure.microsoft.com/en-us/) and provide your own SignalR connection string.

* `"AzureSignalRConnectionString": ""`    

For client side instructions please see [Subscribing to events from the client - Option 3 or 4](./#i-am-using-option-3-or-4).

### Option \#4 - Using the endpoint from [TezosLive.io](https://tezoslive.io)  \(most convenient\)

Sign in using your GitHub account on [TezosLive.io](https://tezoslive.io) and request your endpoint. 

{% hint style="info" %}
You don't need to host anything on server side.
{% endhint %}

API is currently limited to

* 20 000 messages per account per day \(1 message is counted for each 64kB in case message has more than 64kB\)
* 20 concurrent connection per account

Please make sure to check the [documentation](https://docs.tezoslive.io/docs-getting-started/docs-using-tezoslive.io-endpoint) for additional information.

For client side instructions please see [Subscribing to events from the client - Option 3 or 4](./#i-am-using-option-3-or-4).

If you need more messages or concurrent connections please contact us _hello AT tezoslive.io._

## Subscribing to events from the client

### I am using option \#1 or \#2

You can connect to the hub for example like this \(see `signalr.service.ts`\)

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
this.hubConnection.send("subscribe", { 
   transactionAddresses: ['all'],
   delegationAddresses: ['all'],
   originationAddresses: ['all']
});
```

Note: `transactionAddresses`, `delegationAddresses` and `originationAdresses` are `string[]`.

{% hint style="info" %}
Specifying **'all'** will subscribe the client to all transactions/delegations/originations respectively.
{% endhint %}

For reference please take a look at [AgileVentures.TezPusher.SampleClient.Web](https://github.com/agile-ventures/TaaS/tree/master/AgileVentures.TezPusher.SampleClient.Web) specifically [`signalr.service.ts`](https://github.com/agile-ventures/TaaS/blob/84fe386b38f5e488a194a2aa531b109c7dc435d6/AgileVentures.TezPusher.SampleClient.Web/src/app/signalr.service.ts#L65).

### I am using option \#3 or \#4

You will need to provide a [UUID](https://en.wikipedia.org/wiki/Universally_unique_identifier) in a custom HTTP header named `x-tezos-live-userid` to identify a client during the initial call to `negotiate` endpoint. In the sample client application we are using the [npm uuid package](https://www.npmjs.com/package/uuid) to generate random UUIDs.

You can see how the subscription to all transactions is being made by looking at the `signalr.service.ts` [here](https://github.com/agile-ventures/TaaS/blob/master/AgileVentures.TezPusher.SampleClient/src/app/signalr.service.ts) by making a `POST` request to `subscribe` endpoint with the following parameters

* userId is `string` - this is the UUID you have used for the `negotiate` call
* `transactionAddresses`, `delegationAddresses` and `originationAddresses`are `string[]` - this is the array of the addresses that you want to subscribe to. You can subscribe to all addresses by sending `['all']`

You can also subscribe only to a subset of addresses, that you are interested in by providing them as a parameter to `subscribe` call. You need to provide the generated UUID that you used in the `negotiate` call along with the array of the addresses.

For reference please take a look at [AgileVentures.TezPusher.SampleClient](https://github.com/agile-ventures/TaaS/tree/master/AgileVentures.TezPusher.SampleClient).

Or you can check out deployed version of this app available here [https://client-staging.tezoslive.io/](https://client-staging.tezoslive.io/).

## Solution Description

Solution consists of several projects described bellow

* [AgileVentures.TezPusher.Function](https://github.com/agile-ventures/TaaS/tree/master/AgileVentures.TezPusher.Function) Azure Function getting the updates from Pusher.ConsoleApp and sending the updates to SignalR hub.
* [AgileVentures.TezPusher.Model](https://github.com/agile-ventures/TaaS/tree/master/AgileVentures.TezPusher.Model) Simple Model for the updates. This will be extended heavily based on the different subscriptions.
* [AgileVentures.TezPusher.Pusher.ConsoleApp](https://github.com/agile-ventures/TaaS/tree/master/AgileVentures.TezPusher.Pusher.ConsoleApp) Small Console Application in .NET Core used to monitor Tezos Node and push updates to the Azure Function.
* [AgileVentures.TezPusher.Pusher.Web](https://github.com/agile-ventures/TaaS/tree/master/AgileVentures.TezPusher.Pusher.Web) ASP.NET Core Application, that monitors Tezos Node and also provides updates to clients through SignalR hub over WebSocket transport.

  **Docker supported!** To try-out docker version you can also get it from Docker Hub here [https://hub.docker.com/r/tezoslive/agileventurestezpusherweb](https://hub.docker.com/r/tezoslive/agileventurestezpusherweb). See instructions for [Option \#1](./#option-1---running-pusherweb-in-docker-most-convenient-at-the-moment).

* [AgileVentures.TezPusher.SampleClient](https://github.com/agile-ventures/TaaS/tree/master/AgileVentures.TezPusher.SampleClient) Sample Client application written in Angular consuming the updates provided by the Azure SignalR hub.
* [AgileVentures.TezPusher.SampleClient.Web](https://github.com/agile-ventures/TaaS/tree/master/AgileVentures.TezPusher.SampleClient.Web) Sample Client application written in Angular consuming the updates provided by the ASP.NET Core SignalR hub.

### Sample Client Applications

* For [Option \#1](./#option-1---running-pusherweb-in-docker-most-convenient-at-the-moment) & [Option \#2](./#option-2---running-pusherweb-as-a-standalone-aspnet-core-app) - [AgileVentures.TezPusher.SampleClient.Web](https://github.com/agile-ventures/TaaS/tree/master/AgileVentures.TezPusher.SampleClient.Web)
* For  [Option \#3](./#option-3---using-azure-functions-and-tezpusherconsoleapp) & [Option \#4](./#option-4---using-the-endpoint-from-tezosliveio) at [https://client-staging.tezoslive.io/](https://client-staging.tezoslive.io/)

