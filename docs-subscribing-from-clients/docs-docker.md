---
description: >-
  If you are using TezPusher.Web in Docker or as a stand-alone ASP.Net Core
  Application.
---

# Clients - Docker

You can connect to the hub for example like this \(see [`signalr.service.ts`](https://github.com/agile-ventures/TaaS/blob/84fe386b38f5e488a194a2aa531b109c7dc435d6/AgileVentures.TezPusher.SampleClient.Web/src/app/signalr.service.ts#L65)\).

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

