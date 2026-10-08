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

Launch `index.html` in your browser.