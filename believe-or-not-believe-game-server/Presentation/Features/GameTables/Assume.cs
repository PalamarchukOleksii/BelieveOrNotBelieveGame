using Domain.Abstractions.GameAbstractions;
using Microsoft.AspNetCore.SignalR;
using Presentation.Constants;
using Presentation.Endpoints;
using Presentation.Extensions;
using Presentation.Hubs;

namespace Presentation.Features.GameTables;

public class Assume
{
    public record Request(string ConnectionId, string GameName, bool IsBelieving);
    
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost($"{EndpointTags.GameTables}/assume", HandleAsync).WithTags(EndpointTags.GameTables);
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
            return Results.BadRequest(
                $"Player with connection ID {request.ConnectionId} don't exists in game {request.GameName}");
        }

        if (!table.CheckIfPlayerCanMakeAssume(player))
        {
            return Results.BadRequest("You can not make assume");
        }
        
        (bool endGame, string message) = table.MakeAssumeOnGameTable(request.IsBelieving);
            
        await hubContext.Clients.Group(table.GameName).SendAsync("ReceiveAssume", message);

        if (endGame)
        {
            await hubContext.SendEndGameTableAsync(table);
        }
        else
        {
            await hubContext.SendGameTableStateAsync(table);
        }

        await hubContext.SendInfoAboutOpponentsAsync(table);
        await hubContext.SendPlayersCardsAsync(table);
            
        return Results.Ok();
    }
}