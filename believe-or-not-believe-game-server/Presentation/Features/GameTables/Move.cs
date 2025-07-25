using Domain.Abstractions.GameAbstractions;
using Domain.Enums;
using Microsoft.AspNetCore.SignalR;
using Presentation.Constants;
using Presentation.Endpoints;
using Presentation.Extensions;
using Presentation.Hubs;

namespace Presentation.Features.GameTables;

public static class Move
{
    public record Request(string ConnectionId, string GameName, string CardsValue, IReadOnlyList<int> CardsId);

    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost($"{EndpointTags.GameTables}/move", HandleAsync).WithTags(EndpointTags.GameTables);
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

        var moveCheckResult = table.CheckIfPlayerCanMakeMove(player, request.CardsId);

        bool isSuccess;
        string message;

        switch (moveCheckResult)
        {
            case MoveCheckResult.CanMakeMove:
                var playerMove = new global::Domain.Models.GameModels.Move(player.Name, request.CardsValue, request.CardsId);
                table.MakeMoveOnGameTable(playerMove);

                isSuccess = true;
                message = $"{playerMove.PlayerName} threw {playerMove.CardsId.Count} {playerMove.CardValue}(s)";
                break;

            case MoveCheckResult.ZeroCards:
                isSuccess = false;
                message = "You can only make an assumption";
                break;

            case MoveCheckResult.NotPlayersTurn:
                isSuccess = false;
                message = "It is not your turn";
                break;

            case MoveCheckResult.DoesNotHaveCards:
                isSuccess = false;
                message = "You don't have these cards";
                break;

            default:
                isSuccess = false;
                message = "Invalid move";
                break;
        }

        if (!isSuccess)
        {
            await hubContext.Clients.Client(request.ConnectionId).SendAsync("RecieveNotMove", message);
            return Results.BadRequest(message);
        }
        
        await hubContext.Clients.Group(request.GameName).SendAsync("RecieveMove", message);

        await hubContext.SendGameTableStateAsync(table);
        await hubContext.SendInfoAboutOpponentsAsync(table);
        await hubContext.SendPlayersCardsAsync(table);
        return Results.Ok();
    }
}