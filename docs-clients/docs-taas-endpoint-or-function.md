---
description: If you are using TezosLive.io Endpoint or Azure Functions.
---

# Clients - TezosLive.io Endpoint or Azure Functions

### Sample Client

For reference please take a look at [AgileVentures.TezPusher.SampleClient](https://github.com/agile-ventures/TaaS/tree/master/AgileVentures.TezPusher.SampleClient).  
Or you can check out deployed version of this app available at [https://client-staging.tezoslive.io/](https://client-staging.tezoslive.io/).

### 1. Getting the url and accessToken from the /api/negotiate

{% page-ref page="../docs-api-endpoints/docs-negotiate.md" %}

### 2. Connecting to the SignalR Hub

{% hint style="warning" %}
You need a SignalR client library. In this sample we are using [https://www.npmjs.com/package/@aspnet/signalr](https://www.npmjs.com/package/@aspnet/signalr).

Your usage may vary depending on your programming language and used client library.
{% endhint %}

Using the data from the `/api/negotiate` response we can now connect to a SignalR hub, where

* `{url}` is the `url` parameter from `/api/negotiate` response call
* `{options}` is the object with `accessTokenFactory` field containing `accessToken` from 

  `/api/negotiate` response call

```text
this.hubConnection = new signalR.HubConnectionBuilder()
                .withUrl({url}, {options})
                .configureLogging(signalR.LogLevel.Information)
                .build();

this.hubConnection.start().catch(err => console.error(err.toString()));
```

You can also check our Sample Client [source code](https://github.com/agile-ventures/TaaS/blob/c961382c1bf5815633da7e1ba0c4865fbe65873e/AgileVentures.TezPusher.SampleClient/src/app/signalr.service.ts#L146) and [SignalR Client Documentation](https://docs.microsoft.com/en-us/aspnet/core/signalr/client-features?view=aspnetcore-3.0).

### 3. Subscribing to updates

{% page-ref page="../docs-api-endpoints/docs-api-subscribe.md" %}

### 4. Unsubscribe from updates

{% page-ref page="../docs-api-endpoints/docs-api-unsubscribe.md" %}

### 

