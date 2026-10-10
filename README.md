# Repro project for SignalR issue

This is an Azure function (C#) project that demonstrates an issue with Azure SignalR Service access token containing claims with non-ascii characters.

## Local setup

This project is set up to run in Visual studio Code.

Additional prerepquisites:
1. [Azure SignalR Local Emulator](https://learn.microsoft.com/en-us/azure/azure-signalr/signalr-howto-emulator) installed
1. [Azurite extension](https://marketplace.visualstudio.com/items?itemName=Azurite.azurite) in VS Code

Before running this locally, create `local.settings.json` file in the root, with the following content:
```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "AzureSignalRConnectionString": "Endpoint=http://localhost;Port=8888;AccessKey=ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789ABCDEFGH;Version=1.0;"
  }
}
```

This file sets up Azure functions project to use locally running SignalR Emulator and Azurite storage.

## Starting the Azure functions

1. Start Azurite Blob storage in VS Code. The Azurite storage files will be put into `.azurite` folder.
1. Start SignalR emulator by executiong `asrs-emulator start`. The `settings.json` file already contains its upstream configuration.
1. Start Azure function by executing `func start`.

## Issue reproduction

1. Launch `index.html` in your browser. Ensure that `SignalR Hub endpoint` value matchees your running Azure function address from above (without the `/negotiate` segment). 
1. When you click `Connect` observe the output in `SignalR Emulator` console. If the `UserId` value in `SognalRFunctions.cs`/`Negotiate` method contains non-asci character, the Emulator will fail to call upstream Azure function with:
```text
warn: Microsoft.Azure.SignalR.Emulator.HttpUpstreamTrigger[0]
      Failed to write message during operation {hub}=demohub,{event}=connected,{category}=connections: System.Net.Http.HttpRequestException: Request headers must contain only ASCII characters.
         at System.Net.Http.HttpConnection.<WriteString>g__ThrowForInvalidCharEncoding|56_0()
         at System.Net.Http.HttpConnection.WriteString(String s, Encoding encoding)
         at System.Net.Http.HttpConnection.WriteHeaderCollection(HttpHeaders headers, String cookiesFromContainer)
         at System.Net.Http.HttpConnection.WriteHeaders(HttpRequestMessage request, HttpMethod normalizedMethod)
         at System.Net.Http.HttpConnection.SendAsync(HttpRequestMessage request, Boolean async, CancellationToken cancellationToken)
         at System.Net.Http.HttpConnection.SendAsync(HttpRequestMessage request, Boolean async, CancellationToken cancellationToken)
         at System.Net.Http.HttpConnectionPool.SendWithVersionDetectionAndRetryAsync(HttpRequestMessage request, Boolean async, Boolean doRequestAuth, CancellationToken cancellationToken)
         at System.Net.Http.DiagnosticsHandler.SendAsyncCore(HttpRequestMessage request, Boolean async, CancellationToken cancellationToken)
         at System.Net.Http.RedirectHandler.SendAsync(HttpRequestMessage request, Boolean async, CancellationToken cancellationToken)
         at Microsoft.Extensions.Http.Logging.LoggingHttpMessageHandler.<SendCoreAsync>g__Core|5_0(HttpRequestMessage request, Boolean useAsync, CancellationToken cancellationToken)
         at Microsoft.Extensions.Http.Logging.LoggingScopeHttpMessageHandler.<SendCoreAsync>g__Core|5_0(HttpRequestMessage request, Boolean useAsync, CancellationToken cancellationToken)
         at System.Net.Http.HttpClient.<SendAsync>g__Core|83_0(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationTokenSource cts, Boolean disposeCts, CancellationTokenSource pendingRequestsCts, CancellationToken originalCancellationToken)
         at Microsoft.Azure.SignalR.Emulator.HttpUpstreamTrigger.SendAsync(HttpRequestMessage request, String operationName, CancellationToken token) in D:\a\_work\1\s\src\Microsoft.Azure.SignalR.Emulator\Upstreams\HttpUpstreamTrigger.cs:line 105
         at Microsoft.Azure.SignalR.Emulator.HttpUpstreamTrigger.SafeAuthAndSendAsync(HttpRequestMessage request, String operationName, CancellationToken token) in D:\a\_work\1\s\src\Microsoft.Azure.SignalR.Emulator\Upstreams\HttpUpstreamTrigger.cs:line 92
```
