using Microsoft.AspNetCore.SignalR.Client;
using Kusto.Data;
using Kusto.Data.Net.Client;
using Kusto.Data.Common;
using Kusto.Ingest;
using System.Data;

/// <summary>
/// Example of using WebSocket, Trimble Notification Service, Kusto SDK, and SignalR to stream messages into Kusto.
/// References: 
/// https://developer.trimblemaps.com/restful-apis/trip-management/notifications-service/
/// https://github.com/Azure/azure-kusto-samples-dotnet
/// </summary>
/// <remarks>
/// This sample assumes: 
/// 1. A valid Trimble API key with the TM add-on.
/// 2. Your Kusto cluster has streaming enabled. (Streaming is enabled by default for Fabric KQL Database & ADX Free-Personal Clusters)
/// 3. Your Kusto database has a table with the following schema:
///     .create table TrimbleNotificationRaw (message:dynamic)
/// </remarks>


// params required
string apiKey = "your-api-key";
string clusterUri = "your-kql-clusterUri";
string database = "your-kql-database";
string table = "your-kql-table"; 


// kusto client settings
var clusterKcsb = new KustoConnectionStringBuilder(clusterUri)
    .WithAadUserPromptAuthentication(); // .WithAadApplicationKeyAuthentication(applicationClientId: "<your-client-id>", applicationKey: "<your-secret>", authority: "your-tenant-id");
var ingestClient = KustoIngestFactory
    .CreateStreamingIngestClient(clusterKcsb);
var ingestProps = new KustoIngestionProperties(database, table) {
    Format = DataSourceFormat.txt
    };


// signalR connection to trimble notification service
var connection = new HubConnectionBuilder()
    .WithUrl("https://notifications.trimblemaps.com/register?apikey=" + apiKey)
    .WithAutomaticReconnect(new [] {TimeSpan.FromSeconds(5)})
    .Build();

    connection.On<dynamic>("notificationMessage", (message) =>
    {
        try
        {
            Console.WriteLine($"message: {message}");
            
            // create a stream from the message
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter( stream );
            stream.Position = 0;
            // write the message to the stream
            writer.Write( message );
            writer.Flush();
            stream.Position = 0;

            // ingest the stream to kusto
            _= ingestClient.IngestFromStreamAsync(stream, ingestProps).Result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex}");
        }
    });

// retry connecting to trimble notification service
bool connected = false;
while (!connected)
{
    try
    {
        connection.StartAsync().Wait();
        Console.WriteLine("Connection started");
        connected = true;
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
        await Task.Delay(new Random().Next(0, 5) * 2000); // delay 0-10s before retrying
    }
}
Console.ReadLine();
await connection.StopAsync();