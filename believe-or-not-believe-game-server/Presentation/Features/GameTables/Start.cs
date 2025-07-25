using Domain.Abstractions.GameAbstractions;
using Microsoft.AspNetCore.SignalR;
using Presentation.Constants;
using Presentation.Endpoints;
using Presentation.Extensions;
using Presentation.Hubs;

namespace Presentation.Features.GameTables;

public static class Start
{
    public record Request(string ConnectionId, string GameName);
    
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost($"{EndpointTags.GameTables}/start", HandleAsync).WithTags(EndpointTags.GameTables);
        }
    }

    public static async Task<IResult> HandleAsync(Request request,
        IHubContext<GameHub> hubContext,
        IGameTableService gameTableService)
    {
        if (!GameHub.ConnectionExists(request.ConnectionId))
        {
            return Results.BadRequest("Invalid connection ID");
        }
        
        var table = gameTableService.GetGameTableByName(request.GameName);
        if (table is null)
        {
            return Results.BadRequest($"Game with name {request.GameName} does not exist");
        }

        var player = table.GetPlayerByConnectionId(request.ConnectionId);
        if (player is null)
        {
            return Results.BadRequest($"Player with connection ID {request.ConnectionId} don't exists in game {request.GameName}");
        }

        int countOfPlayersInOnGameTable = table.Players.Count;
        if (countOfPlayersInOnGameTable < 2)
        {
            return Results.BadRequest($"Two players required to start the game {request.GameName}");
        }

        bool result = table.StartGameTable(player);
        if (result)
        {
            await hubContext.Clients.Group(request.GameName).SendAsync("RecieveGameStarted", "Game started");

            await hubContext.SendInfoAboutOpponentsAsync(table);
            await hubContext.SendPlayersCardsAsync(table);
        }
        else
        {
            await hubContext.Clients.Group(request.GameName).SendAsync("RecieveGameNotStarted", "Waiting for other players to start the game");
        }
        
        return Results.Ok();
    }
}