
# TaaS (Tezos as a Service) [STAGING]
TaaS provides real-time updates to various applications from the Tezos Blockchain by leveraging SignalR. 

Solution consists of several projects described bellow

 - [AgileVentures.TezPusher.Function](https://github.com/agile-ventures/TaaS/tree/staging/AgileVentures.TezPusher.Function "AgileVentures.TezPusher.Function")
Azure Function getting the updates from Pusher and sending the updates to SignalR hub.

 - [AgileVentures.TezPusher.Model](https://github.com/agile-ventures/TaaS/tree/staging/AgileVentures.TezPusher.Model "AgileVentures.TezPusher.Model")
Simple Model for the updates. This will be extended heavily based on the different subscriptions.

 - [AgileVentures.TezPusher.Pusher](https://github.com/agile-ventures/TaaS/tree/staging/AgileVentures.TezPusher.Pusher "AgileVentures.TezPusher.Pusher")
Small Console Applications in .NET Core used to monitor Tezos Node and push updates to the Azure Function.

 - [AgileVentures.TezPusher.SampleClient](https://github.com/agile-ventures/TaaS/tree/staging/AgileVentures.TezPusher.SampleClient "AgileVentures.TezPusher.SampleClient")
 Sample Client application written in Angular consuming the updates provided by the SignalR hub.   

## Sample Client Application
Deployed at [https://client-staging.tezoslive.io/](https://client-staging.tezoslive.io/)

## How to use
You will need to provide a [UUID](https://en.wikipedia.org/wiki/Universally_unique_identifier) to identify a client during the initial call to `negotiate` endpoint. In the sample client application we are using the [npm uuid package](https://www.npmjs.com/package/uuid) to generate random UUIDs. 

You can see how the subscription to all transactions is being made by looking at the `signalr.service.ts` [here](https://github.com/agile-ventures/TaaS/blob/master/AgileVentures.TezPusher.SampleClient/src/app/signalr.service.ts). 

You can also subscribe only to a subset of addresses, that you are interested in by providing them as a parameter to `subscribe` call. You need to provide the generated UUID that you used in the `negotiate` call along with the array of the addresses.

