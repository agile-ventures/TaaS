---
description: >-
  If you want to use Serverless Azure Functions and Azure SignalR Service with
  TaaS
---

# Using Docker with Azure Functions and SignalR Service

## Running TaaS with Azure Functions and Azure SignalR Service

{% hint style="danger" %}
**Requirements**

* Azure Account \([https://azure.microsoft.com/en-us/free/](https://azure.microsoft.com/en-us/free/)\)
* Docker
* Tezos Node with enabled RPC endpoint supporting following calls
  * _/monitor/heads/main_
  * _/chains/main/blocks/hash_
{% endhint %}

In this configuration we are using a different docker image - this one is only sending a new blocks to our Azure Function, which then sends parsed information to subscribers through Azure SignalR Service.

{% hint style="info" %}
If you just want to use a Docker container without having to setup Azure infrastructure please check [Using Docker](docs-using-docker.md) documentation section.
{% endhint %}

Ready-to-use docker image is available from Docker Hub here:   
[https://hub.docker.com/repository/docker/tezoslive/agileventurestezpusherconsoleapp](https://hub.docker.com/repository/docker/tezoslive/agileventurestezpusherconsoleapp)

Example of the `docker run` command

* setting the `Tezos:NodeUrl` env. variable to https://172.17.0.1:8732
* setting the `Azure:AzureFunctionUrl` env. variable to [https://myfunction.azurewebsites.net](https://myfunction.azurewebsites.net)
* setting the `Azure:AzureFunctionKey` env. variable to MySecretFunctionKey

{% hint style="warning" %}
Be sure to configure the following ENV keys correctly per your environment

* `Tezos:NodeUrl`
* `Azure:AzureFunctionUrl`
* `Azure:AzureFunctionKey`
{% endhint %}

```text
docker run -it --env Tezos:NodeUrl="https://172.17.0.1:8732" \
--env Azure:AzureFunctionUrl="https://myfunction.azurewebsites.net" \
--env Azure:AzureFunctionKey="MySecretFunctionKey" \
 tezoslive/agileventurestezpusherconsoleapp
```

This infrastructure setup has the following benefits 

* It allows you to have a more robust security out of the box as all communication is encrypted by TLS. 
* It makes scaling your applications for thousands of subscribers much easier by using serveless compute with scalable SignalR Service.

**For client side instructions** please see [Subscribing to events from the client - Option 3 or 4](../#i-am-using-option-3-or-4).

{% hint style="info" %}
More information about **Azure Functions** can be found at [https://azure.microsoft.com/en-us/services/functions/](https://azure.microsoft.com/en-us/services/functions/).

More information about **Azure SignalR Service** can be found at [https://azure.microsoft.com/en-us/services/signalr-service/](https://azure.microsoft.com/en-us/services/signalr-service/).
{% endhint %}

