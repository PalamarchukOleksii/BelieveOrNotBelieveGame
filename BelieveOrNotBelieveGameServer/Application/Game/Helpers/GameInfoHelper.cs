using BelieveOrNotBelieveGameServer.Dtos;
using BelieveOrNotBelieveGameServer.Models;
using Microsoft.AspNetCore.SignalR;

namespace BelieveOrNotBelieveGameServer.Application.Game.Helpers;

public static class GameInfoHelper
{
    public static async Task SendPlayersCards(GameTable gameTable, IHubCallerClients clients)
    {
        for (int i = 0; i < gameTable.Players.Count; i++)
        {
            await clients.Client(gameTable.Players[i].PlayerConnectionId).SendAsync("ReceiveCard", gameTable.Players[i].PlayersCards);
        }
    }

    public static async Task SendInfoAboutOpponentsAsync(GameTable gameTable, IHubCallerClients clients)
    {
        List<OpponentInfoDto> opponentsInfo = gameTable.Players
            .Select(p => new OpponentInfoDto
            {
                Name = p.Name,
                CardCount = p.PlayersCards.Count
            })
            .ToList();

        foreach (var player in gameTable.Players)
        {
            await clients.Client(player.PlayerConnectionId).SendAsync("ReceiveOpponentsInfo", opponentsInfo.Where(x => x.Name != player.Name));
        }

        foreach (var playerWhoWin in gameTable.PlayersWhoWin)
        {
            await clients.Client(playerWhoWin.PlayerConnectionId).SendAsync("ReceiveOpponentsInfo", opponentsInfo.Where(x => x.Name != playerWhoWin.Name));
        }
    }
}
