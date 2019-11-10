---
description: If you want to host your TaaS endpoint yourself in Docker.
---

# Using Docker

## Running TaaS in Docker

{% hint style="danger" %}
**Requirements**

* Docker
* Tezos Node with enabled RPC endpoint supporting following calls
  * _/monitor/heads/main_
  * _/chains/main/blocks/hash_
{% endhint %}

Ready-to-use docker image is available from Docker Hub here: [https://hub.docker.com/r/tezoslive/agileventurestezpusherweb](https://hub.docker.com/r/tezoslive/agileventurestezpusherweb).

Example of the `docker run` command

* exposing port 80
* setting the Tezos:NodeUrl environment variable to http://172.17.0.1:8732

{% hint style="warning" %}
Do not forget to change the **Tezos:NodeUrl** based on your configuration!
{% endhint %}

```text
docker run --rm -it -p 80:80 \
--env Tezos:NodeUrl="http://172.17.0.1:8732" \
tezoslive/agileventurestezpusherweb
```

#### Optional Configuration

{% hint style="info" %}
By providing ENV variable **Logging:LogLevel:Default** you can configure logging level.

* Trace
* Debug
* Information
* Warning
* Error
* Critical
{% endhint %}

**For client side instructions** please see [Subscribing to events from the client - Option 1 or 2](../#i-am-using-option-1-or-2).

#### HTTPS support in TaaS Docker image

{% hint style="warning" %}
If you are considering opening up your ports to the public you should configure a certificate and only expose **HTTPS** endpoint to the outside.
{% endhint %}

Example of the `docker run` command for Linux

* exposing ports 8000 \(http\) and 8001\(https\)
* setting certificate path and password

```text
docker run --rm -it -p 8000:80 -p 8001:443 -e ASPNETCORE_URLS="https://+;http://+" -e ASPNETCORE_HTTPS_PORT=8001 -e ASPNETCORE_Kestrel__Certificates__Default__Password="password" -e ASPNETCORE_Kestrel__Certificates__Default__Path=/https/aspnetapp.pfx -v ${HOME}/.aspnet/https:/https/  tezoslive/agileventurestezpusherweb
```

For further information about setting up the certificates please refer to [https://docs.microsoft.com/en-us/aspnet/core/security/docker-https?view=aspnetcore-3.0](https://docs.microsoft.com/en-us/aspnet/core/security/docker-https?view=aspnetcore-3.0).

For development with Docker over HTTPS please refer to [https://github.com/dotnet/dotnet-docker/blob/master/samples/aspnetapp/aspnetcore-docker-https-development.md](https://github.com/dotnet/dotnet-docker/blob/master/samples/aspnetapp/aspnetcore-docker-https-development.md).

