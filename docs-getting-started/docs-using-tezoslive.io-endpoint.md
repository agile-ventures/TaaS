---
description: 'Most convenient option, no server side infrastructure needed.'
---

# Using TezosLive.io Endpoint

## Getting your TaaS Endpoint at TezosLive.io

{% hint style="danger" %}
**Requirements**

* GitHub account
{% endhint %}

Sign in using your GitHub account on [TezosLive.io](https://www.tezoslive.io) and request your endpoint.   
You don't need to setup or host any server-side or Tezos infrastructure on your side. 

You can also check the [Medium article ](https://medium.com/tezoslive/public-tezos-signalr-websocket-endpoint-available-on-tezoslive-io-28e0dcfcc8f)about the public endpoints.

{% hint style="warning" %}
**Limitations**

Public API endpoints are currently limited to

* 20 000 messages per account per day  \(1 message is counted for each 64kB in case message has more than 64kB\)
* 20 concurrent connection per account
{% endhint %}

**For client side instructions** please see [Subscribing to events from the client - Option 3 or 4](../#i-am-using-option-3-or-4).



