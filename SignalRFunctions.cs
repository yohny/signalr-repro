using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Repro;

public class SignalRFunctions
{
    private readonly ILogger _logger;

    public SignalRFunctions(ILogger<SignalRFunctions> logger)
    {
        _logger = logger;
    }

    [Function(nameof(Negotiate))]
    public IActionResult Negotiate(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req,
        [SignalRConnectionInfoInput(HubName = "demohub", UserId = "ascii-only-value")] SignalRConnectionInfo connectionInfo)
    {
        _logger.LogInformation("SignalR negotiation called with URL: {url}, AccessToken: {accessToken}", connectionInfo.Url, connectionInfo.AccessToken);
        return new JsonResult(connectionInfo);

    }

    [Function(nameof(OnClientConnected))]
    public static void OnClientConnected(
        [SignalRTrigger("demohub", "connections", "connected")]
        SignalRInvocationContext invocationContext, FunctionContext functionContext)
    {
        var logger = functionContext.GetLogger(nameof(OnClientConnected));
        logger.LogInformation(
            "SignalR client connected. ConnectionId: {connectionId}, UserId: {userId}",
            invocationContext.ConnectionId,
            invocationContext.UserId);
    }

    [Function(nameof(OnClientDisconnected))]
    public static void OnClientDisconnected(
        [SignalRTrigger("demohub", "connections", "disconnected")]
        SignalRInvocationContext invocationContext, FunctionContext functionContext)
    {
        var logger = functionContext.GetLogger(nameof(OnClientDisconnected));
        logger.LogInformation(
            "SignalR client disconnected. ConnectionId: {connectionId}, UserId: {userId}",
            invocationContext.ConnectionId,
            invocationContext.UserId);
    }
}