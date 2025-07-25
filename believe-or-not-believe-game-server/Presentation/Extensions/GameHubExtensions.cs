using Domain.Models.GameModels;
using Microsoft.AspNetCore.SignalR;
using Presentation.Hubs;

namespace Presentation.Extensions;

public static class GameHubExtensions
{
    public static async Task SendInfoAboutOpponentsAsync(this IHubContext<GameHub> hubContext, GameTable table)
    {
        var opponentInfo = table.GetShortInfoAboutPlayers();
        var (playersConnectionIds, playersWhoWinConnectionIds) = table.GetAllConnectionId();

        foreach (var playerConnectionId in playersConnectionIds)
            await hubContext.Clients.Client(playerConnectionId)
                .SendAsync("ReceiveOpponentsInfo",
                    opponentInfo.Where(x => x.PlayerConnectionId != playerConnectionId));

        foreach (var playerWhoWinConnectionId in playersWhoWinConnectionIds)
            await hubContext.Clients.Client(playerWhoWinConnectionId)
                .SendAsync("ReceiveOpponentsInfo", opponentInfo);
    }

    public static async Task SendPlayersCardsAsync(this IHubContext<GameHub> hubContext, GameTable table)
    {
        var playerCards = table.GetPlayerCards();

        foreach (var player in playerCards)
            await hubContext.Clients.Client(player.PlayerConnectionId).SendAsync("ReceiveCard", player.Cards);
    }

    public static async Task SendGameTableStateAsync(this IHubContext<GameHub> hubContext, GameTable table)
    {
        var gameState = table.GetGameState();

        if (gameState is { CurrentPlayerCanMakeMove: true, CurrentPlayerCanMakeAssume: true })
        {
            await hubContext.Clients.Client(gameState.CurrentMovePlayerConnectionId)
                .SendAsync("ReceiveNextMoveAssume", "You make assume or move");
            await hubContext.Clients.GroupExcept(table.GameName, gameState.CurrentMovePlayerConnectionId)
                .SendAsync("ReceiveCurrentMovePlayer", $"{gameState.CurrentMovePlayerName} make assume or move");
        }
        else if (gameState.CurrentPlayerCanMakeAssume)
        {
            await hubContext.Clients.Client(gameState.CurrentMovePlayerConnectionId)
                .SendAsync("ReceiveNextAssume", "You make assume");
            await hubContext.Clients.GroupExcept(table.GameName, gameState.CurrentMovePlayerConnectionId)
                .SendAsync("ReceiveCurrentMovePlayer", $"{gameState.CurrentMovePlayerName} make assume");
        }
        else
        {
            await hubContext.Clients.Client(gameState.CurrentMovePlayerConnectionId)
                .SendAsync("ReceiveNextMove", "You make move");
            await hubContext.Clients.GroupExcept(table.GameName, gameState.CurrentMovePlayerConnectionId)
                .SendAsync("ReceiveCurrentMovePlayer", $"{gameState.CurrentMovePlayerName} make move");
        }

        await hubContext.Clients.Group(table.GameName).SendAsync("ReceiveCardOnTableCount", gameState.CardsOnTableCount);
        await hubContext.Clients.Group(table.GameName).SendAsync("ReceiveMakeMoveValues", gameState.MakeMoveValue);
    }
}