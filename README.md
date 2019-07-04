
# TaaS
Tezos as a Service

Solution includes several projects described bellow

 - [AgileVentures.TezPusher.Function](https://github.com/agile-ventures/TaaS/tree/master/AgileVentures.TezPusher.Function "AgileVentures.TezPusher.Function")
Azure Function getting the updates from Pusher and sending the updates to SignalR hub.

 - [AgileVentures.TezPusher.Model](https://github.com/agile-ventures/TaaS/tree/master/AgileVentures.TezPusher.Model "AgileVentures.TezPusher.Model")
Simple Model for the updates. This will be extended heavily based on the different subscriptions.

 - [AgileVentures.TezPusher.Pusher](https://github.com/agile-ventures/TaaS/tree/master/AgileVentures.TezPusher.Pusher "AgileVentures.TezPusher.Pusher")
Small Console Applications in .NET Core used to monitor Tezos Node and push updates to the Azure Function. This project will be updated to include Docker support.

 - [AgileVentures.TezPusher.SampleClient](https://github.com/agile-ventures/TaaS/tree/master/AgileVentures.TezPusher.SampleClient "AgileVentures.TezPusher.SampleClient")
 Sample Client application written in Angular consuming the updates provided by the SignalR hub.   
