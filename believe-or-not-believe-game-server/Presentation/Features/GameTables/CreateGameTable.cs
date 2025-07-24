using Domain.Abstractions.GameAbstractions;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Presentation.Endpoints;
using Presentation.Hubs;

namespace Presentation.Features.GameTables;

public static class CreateGameTable
{
    public record Request(
        string ConnectionId,
        string CreatorName,
        string GameName,
        int NumOfCards,
        int MaxNumOfPlayers,
        bool AddBot);

    public record Response(string GameName, int NumOfCards, int MaxNumOfPlayers, bool AddBot);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("game-table/create", HandleAsync).WithTags(EndpointTags.GameTable);
        }
    }

    public static async Task<IResult> HandleAsync(
        Request request,
        IHubContext<GameHub> hubContext,
        IGameTableService gameTableService,
        ILogger<CreateGameTable.Endpoint> logger)
    {
        logger.LogInformation("Received request to create game table: {@Request}", request);
        
        if (!GameHub.ConnectionExists(request.ConnectionId))
        {
            logger.LogWarning("Connection ID {ConnectionId} does not exist or is not connected", request.ConnectionId);
            return Results.BadRequest("Invalid connection ID");
        }

        var table = gameTableService.GetGameTableByName(request.GameName);
        if (table is not null)
        {
            logger.LogWarning("Game with name {GameName} already exists", request.GameName);
            return Results.BadRequest($"Game with name {request.GameName} already exist");
        }

        table = gameTableService.CreateGameTable(
            request.GameName,
            request.NumOfCards,
            request.MaxNumOfPlayers,
            request.AddBot);

        if (table is null)
        {
            logger.LogError("Failed to create game table for {GameName}", request.GameName);
            return Results.InternalServerError();
        }

        logger.LogInformation("Game table {GameName} created successfully", table.GameName);
        await hubContext.Clients.All.SendAsync("RecieveNewGameCreated",
            new Response(table.GameName, table.NumOfCards, table.MaxNumOfPlayers, table.AddBot));

        var result = table.JoinGameTable(request.CreatorName, request.ConnectionId);
        if (!result)
        {
            logger.LogError("Failed to join game table {GameName} for user {CreatorName}", request.GameName, request.CreatorName);
            return Results.InternalServerError();
        }

        await hubContext.Groups.AddToGroupAsync(request.ConnectionId, request.GameName);
        logger.LogInformation("User {CreatorName} with connection ID {ConnectionId} joined game {GameName}",
            request.CreatorName, request.ConnectionId, request.GameName);

        return Results.Ok();
    }
}
