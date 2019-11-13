# /api/unsubscribe

{% api-method method="post" host="https://{yourEndpoint}" path="/api/unsubscribe" %}
{% api-method-summary %}
Unsubscribe
{% endapi-method-summary %}

{% api-method-description %}
This method allows the client to unsubscribe from various update types. 
{% endapi-method-description %}

{% api-method-spec %}
{% api-method-request %}
{% api-method-headers %}
{% api-method-parameter name="Content-Type" type="string" required=false %}
application/json
{% endapi-method-parameter %}
{% endapi-method-headers %}

{% api-method-body-parameters %}
{% api-method-parameter name="originationAddresses" type="string" required=false %}
Unsubscribe to origination happening on these addresses.
{% endapi-method-parameter %}

{% api-method-parameter name="delegationAddresses" type="string" required=false %}
Unsubscribe to delegation happening on these addresses.
{% endapi-method-parameter %}

{% api-method-parameter name="transactionAddresses" type="string" required=false %}
Unsubscribe to transactions happening on these addresses.
{% endapi-method-parameter %}

{% api-method-parameter name="userId" type="string" required=true %}
UUID used in `/api/negotiate`
{% endapi-method-parameter %}
{% endapi-method-body-parameters %}
{% endapi-method-request %}

{% api-method-response %}
{% api-method-response-example httpCode=200 %}
{% api-method-response-example-description %}

{% endapi-method-response-example-description %}

```

```
{% endapi-method-response-example %}
{% endapi-method-response %}
{% endapi-method-spec %}
{% endapi-method %}

