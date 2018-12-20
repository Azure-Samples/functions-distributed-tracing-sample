# Distributed Tracing sample for Azure Functions, and Java with Application Insights

Sample project for developing Distributed Tracing. We are using these technologies. 

* Azure Functions V2
* Storage Queue
* Spring boot
* Service Bus
* Durable Functions (Next Step) 
* Application Insights

## Features

You can learn how to implement Distributed Tracing using Application Insights and Azure Functions. 

```
Storage Queue -> Azure Functions (1) -> Http Request -> Spring boot -> Service Bus -> Azure Functions (2) -> Service Bus -> Azure Functions (3) 
```

We plan to implement two protocol. 

* HttpCorrelationProtocol
* W3C Trace Context

