---
description: >-
  If you are using TezPusher.Web in Docker or as a stand-alone ASP.Net Core
  Application.
---

# Clients - Docker

### Sample Client

For reference please take a look at [AgileVentures.TezPusher.SampleClient.Web](https://github.com/agile-ventures/TaaS/tree/master/AgileVentures.TezPusher.SampleClient.Web).

### 1. Connect to SignalR Hub

You can connect to the hub for example like this \(see [`signalr.service.ts`](https://github.com/agile-ventures/TaaS/blob/84fe386b38f5e488a194a2aa531b109c7dc435d6/AgileVentures.TezPusher.SampleClient.Web/src/app/signalr.service.ts#L65)\).

{% hint style="warning" %}
You need a SignalR client library. In this sample we are using [https://www.npmjs.com/package/@aspnet/signalr](https://www.npmjs.com/package/@aspnet/signalr).

Your usage may vary depending on your programming language and used client library.
{% endhint %}

```typescript
private  connect():  Observable<any> {
    this.hubConnection  = new signalR.HubConnectionBuilder()
        .withUrl(`${this._baseUrl}/tezosHub`)
        .configureLogging(signalR.LogLevel.Information)
        .build();
    return  from(this.hubConnection.start());
}
```

### 2. Subscribe to updates

You can then subscribe to events like this. You can only specify the event types you are interested in. 

```typescript
this.hubConnection.send("subscribe", { 
   transactionAddresses: ['all'],
   delegationAddresses: ['all'],
   originationAddresses: ['all'],
   fromBlockLevel: 744190,
   blockHeaders: true
});
```

Note: `transactionAddresses`, `delegationAddresses` and `originationAdresses` are `string[]`.

{% hint style="warning" %}
If your client missed some blocks and you want to continue where you left off, you can optionally specify **fromBlockLevel** parameter. This will first send all the subscribed information from the blocks starting at the specified block level.
{% endhint %}

{% hint style="info" %}
* Specifying **'all'** will subscribe the client to all transactions/delegations/originations respectively.
* Using specific addresses in the arrays will only subscribe to events happening on these addresses.
{% endhint %}

For reference please take a look at [AgileVentures.TezPusher.SampleClient.Web](https://github.com/agile-ventures/TaaS/tree/master/AgileVentures.TezPusher.SampleClient.Web) specifically [`signalr.service.ts`](https://github.com/agile-ventures/TaaS/blob/84fe386b38f5e488a194a2aa531b109c7dc435d6/AgileVentures.TezPusher.SampleClient.Web/src/app/signalr.service.ts#L65).

### 3. Unsubscribe from updates

You can unsubscribe from events similarly to subscribing.

```typescript
this.hubConnection.send("unsubscribe", { 
   transactionAddresses: ['all'],
   delegationAddresses: ['all'],
   originationAddresses: ['all']
});
```



