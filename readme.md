# Trimble2Kusto 

## Purpose 
Ingest real-time data from Trimble via SignalR/WebSocket into Kusto aka. [Azure Data Explorer](https://aka.ms/adx.pp), [Synapse Data Explorer](https://learn.microsoft.com/azure/synapse-analytics/data-explorer/data-explorer-overview) dedicated-pool, and Microsoft Fabric [KQL Database](https://learn.microsoft.com/fabric/real-time-analytics/create-database) in [Real-Time Intelligence](http://aka.ms/rti-blog). 

## Description
This repo contains sample DotNet Console App [program](notificationsvc/Program.cs) in C# that uses WebSocket, SignalR, Kusto SDK. The program will connect to the [Trimble Notification Service](https://developer.trimblemaps.com/restful-apis/trip-management/notifications-service), provided a valid api-key, via WebSocket and streams messages (service-telemetry, GPS, ETA, OoC, Weather, etc.) into Kusto using DotNet SDK client libraries for real-time analytics. 

The sample assets in this repo are not production solutions. They are samples for proof-of-concept or minimal-viable-product at best. The pipelines establish a data workflow for analytics end2end, including dashboard visualizations. 

If you're wondering, what is Kusto, check out these videos:
1. [The mechanics of Real-Time Intelligence in Microsoft Fabric](<https://youtube.com/watch?v=4h6Wnc294gA>)
2. [Real-Time Intelligence in Microsoft Fabric](<https://youtube.com/watch?v=ugb_w3BTZGE>)
3. [ADX at a glance](https://youtu.be/9rwbsZDD9fw?si=6iIJAPIBVMuvPYIp)
4. [How customers are using it?](https://youtu.be/lOO0PMX3qIk?si=01WXCkWITub0l8RH)

### Summary of Console App
Example of using WebSocket, Trimble Notification Service, Kusto SDK, and SignalR to stream messages into Kusto.

### Dependencies
- .NET 8.0.201
- NuGet packages:
  - Microsoft.AspNetCore.SignalR.Client 
  - Microsoft.Azure.Kusto.Data 
  - Microsoft.Azure.Kusto.Ingest

For your local IDE, recommend using VSCode with extensions C# Dev Kit, and Nuget Package Manager. Install the package dependencies using PowerShell terminal in VSCode by entering command: `dotnet add package <package-name>`

Additionally, StreamingIngestionSample console app is included in this repo to test ingestion & Kusto client libraries using fictitious data. 

### Additional resources
- https://github.com/Azure/azure-kusto-samples-dotnet
- https://learn.microsoft.com/azure/data-explorer/kusto/api/client-libraries
- https://aka.ms/fabric-learn
- https://aka.ms/fabric-docs-rta
- https://aka.ms/adx.youtube
- https://code.visualstudio.com/learn
- https://aka.ms/learn.kql
  

### Remarks
This sample [Program.cs](notificationsvc/Program.cs) assumes: 
1. A valid Trimble API key with the TM add-on.
2. Your Kusto cluster has streaming enabled. (Streaming is enabled by default for Fabric KQL Database & ADX Free-Personal Clusters)
3. Your Kusto database has a table with the following schema: 
```
.create table TrimbleNotificationRaw (message:dynamic)
```

### Running [Program.cs](notificationsvc/Program.cs)
1. Add required DotNet packages. 
2. Enter the required parameters, then build the program.
3. Deploy the program as an [Azure Function](https://azure.microsoft.com/products/functions) when ready.
4. Sample Azure Function - [SignalRToKusto](https://github.com/hfleitas/SignalRToKusto)


## Conceptual Pipelines
1. **Batching**: SQL on-prem > csv file contains +1K "OFD/Active TripIds" (Query & File-transfer) > GET TrimbleAPI TripEvents (using Powershell) > store response in a file per Trip > Load local files to Fabric KQL Database using Get Data > run KQL queries & stored functions to flatten data for analytics > visualize query results in PBI or RTA Dashboard > Apply PBI filters for Dashboard use and parameters for RTA Dashboard.

   in summary...

   SQL > csv file > Trimble API > json files > Fabric KQL DB > KQL queries > PBI + RTA Dashboard.

2. **Streaming**: Trimble Notification Service > C# Client App (via Streaming) > Fabric KQL DB > KQL queries > Dashboards (TBD).


## DISCLAIMER
This Sample Code is provided for the purpose of illustration only and is not intended to be used in a production environment. THIS SAMPLE CODE AND ANY RELATED INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE. We grant You a nonexclusive, royalty-free right to use and modify the Sample Code for your internal use only, provided that You agree: (i) to not use Our name, logo, or trademarks to market Your software product in which the Sample Code is embedded; (ii) to include a valid copyright notice on Your software product in which the Sample Code is embedded; and (iii) to indemnify, hold harmless, and defend Us and Our suppliers from and against any claims or lawsuits, including attorneys' fees, that arise or result from the use or distribution of the Sample Code.
