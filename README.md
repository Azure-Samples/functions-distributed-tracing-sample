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

## How to use

### Prerequisite

#### Azure Functions V2 development Enviornment wich C#

* [Create your first function using Visual Studio](https://docs.microsoft.com/en-us/azure/azure-functions/functions-create-your-first-function-visual-studio)
* [Create your first function using Visual Studio Code](https://docs.microsoft.com/en-us/azure/azure-functions/functions-create-first-function-vs-code)

#### Storage Emulator

For local development, Use Storage Emulator. Start the emulator.

* [Use the Azure storage emulator for development and testing](https://docs.microsoft.com/en-us/azure/storage/common/storage-use-emulator)


If you are using Mac, I recommend to use [Azurite](https://github.com/Azure/Azurite) as a storage emulator. 

#### Java SE Development Kit 8 

Download and install it.

* [Java SE Development Kit 8](https://www.oracle.com/technetwork/java/javase/downloads/jdk8-downloads-2133151.html)

### Start application

#### Create an Application Insights on Azure Portal

Create an Application Insights on Azure Protal. 
Then get InstrumentationKey. 

#### Create local.settings.json

Copy the `local.settings.json.example` to `local.settings.json` then chenge the value of `APPINSIGHTS_INSTRUMENTATIONKEY` to fit your InstrumentationKey of the ApplicationInsights. 

#### Start Azure Functions

Start the Azure Functions. In case of Visual Studio, Hit F5. In case of Visual Studio Code, Open Visual Studio Code on the FunctionsSample/FunctionsSample directory. Then click Debug `Attach to C# Functions`.

#### Create ApplicationInsights.xml

Copy the ApplicationInsights.xml then fill the InstrumentKey part from your Application Insights instrument key. Also, I recommend to create set_environment.sh from the set_environment.sh.example. However, ApplicationInsights.xml might be enough.

#### Start Java Application

Go to the JavaSample directory, then `gradlew bootRun` 

#### Send request to the first function.

Just send get request by postman or something. 

```
http://localhost:7071/api/StartOperation
```

You will see the telemetry is sending to the Application Insights, 
and you will see the Parent Request-Id on the Java Application console and current RequestTelemtryId on your terminal. 

```
Parent Request-Id: |gqHOHSfsTMY=.bc503bfa_1.bc503c00_1.
Current Request Id: |gqHOHSfsTMY=.bc503bfa_1.bc503c00_1.5f667911_ RootId: gqHOHSfsTMY= ParentId: |gqHOHSfsTMY=.bc503b
fa_1.bc503c00_1.
```

like this. After 5 min, you can find the telemetry is on your Application Insigts, Application Map.

# TODO

Now we almost done. Generate Request/Dependency Telemetry and send it to the Application Insights and make sure it works. However something is remains. 

* Sending ParentId to the ServiceBus
* Create a Service Bus Trigger Azure Functions and send the following ServiceBus trigger functions.
* W3C TraceContext correlation sample