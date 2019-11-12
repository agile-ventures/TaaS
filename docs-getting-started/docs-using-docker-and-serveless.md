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

In this configuration we are using a different docker image - this one is only sending a new blocks to our Azure Function which then sends parsed information to subscribers through SignalR Service.

{% hint style="info" %}
If you just want to use a Docker container without having to setup Azure infrastructure please check [Using Docker](docs-using-docker.md) documentation section.
{% endhint %}

