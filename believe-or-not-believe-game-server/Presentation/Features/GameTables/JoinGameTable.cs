using Domain.Abstractions.GameAbstractions;
using Microsoft.AspNetCore.SignalR;
using Presentation.Endpoints;
using Presentation.Hubs;

namespace Presentation.Features.GameTables;

public static class JoinGameTable
{
    public record Request(string ConnectionId, string Username, string GameName);
    
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("game-table/join", HandleAsync).WithTags(EndpointTags.GameTable);
        }
    }

    public static async Task<IResult> HandleAsync(
        Request request,
        IHubContext<GameHub> hubContext,
        IGameTableService gameTableService,
        ILogger<JoinGameTable.Endpoint> logger)  // add logger here
    {
        if (!GameHub.ConnectionExists(request.ConnectionId))
        {
            logger.LogWarning("Connection ID {ConnectionId} does not exist or is not connected", request.ConnectionId);
            return Results.BadRequest("Invalid connection ID");
        }
    
        var table = gameTableService.GetGameTableByName(request.GameName);
        if (table is null)
        {
            logger.LogWarning("Game table {GameName} does not exist", request.GameName);
            return Results.BadRequest($"Game with name {request.GameName} does not exist");
        }

        var player = table.GetPlayerByName(request.Username);
        if (player is not null)
        {
            logger.LogWarning("Player with username {Username} already exists in game {GameName}", request.Username, request.GameName);
            return Results.BadRequest($"Player with username {request.Username} already exists in game {request.GameName}");
        }
    
        var result = table.JoinGameTable(request.Username, request.ConnectionId);
        if (!result)
        {
            logger.LogError("Failed to join game table {GameName} for user {Username}", request.GameName, request.Username);
            return Results.InternalServerError();
        }
    
        await hubContext.Groups.AddToGroupAsync(request.ConnectionId, request.GameName);
        logger.LogInformation("User {Username} with connection ID {ConnectionId} joined game {GameName}",
            request.Username, request.ConnectionId, request.GameName);

        await hubContext.Clients.GroupExcept(request.GameName, request.ConnectionId)
            .SendAsync("ReceiveJoin", $"Player {request.Username} joined game");
        
        //await hubContext.SendInfoAboutOpponents(gameName);

        return Results.Ok();
    }

}