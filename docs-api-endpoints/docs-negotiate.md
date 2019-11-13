# /api/negotiate

{% api-method method="get" host="https://{yourEndpoint}" path="/api/negotiate" %}
{% api-method-summary %}
Negotiate
{% endapi-method-summary %}

{% api-method-description %}
Calling the `negotiate` endpoint along with generated UUID in `x-tezos-live-userid` HTTP header will return `url` and `accessToken`in the response, which you will use for SignalR hub connection.
{% endapi-method-description %}

{% api-method-spec %}
{% api-method-request %}
{% api-method-headers %}
{% api-method-parameter name="x-tezos-live-userid" type="string" required=true %}
UUID client identification
{% endapi-method-parameter %}
{% endapi-method-headers %}
{% endapi-method-request %}

{% api-method-response %}
{% api-method-response-example httpCode=200 %}
{% api-method-response-example-description %}

{% endapi-method-response-example-description %}

```
{
    "url":"https://{yourEndpoint}.signalr.net/client/?hub=broadcast",
    "accessToken":"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9"
}
```
{% endapi-method-response-example %}
{% endapi-method-response %}
{% endapi-method-spec %}
{% endapi-method %}

{% hint style="info" %}
You will need the

* `url` Response parameter for SignalR hub connection
* `accessToken` Response parameter for SignalR hub connection
* `x-tezos-live-userid` parameter for [Subscribe](docs-api-subscribe.md)/[Unsubscribe ](docs-api-unsubscribe.md)calls
{% endhint %}

For generating UUIDs you can use any library, that complies with [RFC4122](https://www.ietf.org/rfc/rfc4122.txt).  
For example [https://www.npmjs.com/package/uuid](https://www.npmjs.com/package/uuid).   
You can also use different method for generating userIds, as long as you can ensure, that each client will have unique id.

