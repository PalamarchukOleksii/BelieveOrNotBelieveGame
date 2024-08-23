using BelieveOrNotBelieveGameServer.Models;
using BelieveOrNotBelieveGameServer.Services.Abstraction;

namespace BelieveOrNotBelieveGameServer.Services;

public class BotService : IBotService
{
    private decimal probapility = 0;

    public void MakeMove(GameTable gameTable)
    {
        var previousPlayer = gameTable.CurrentMovePlayer;
        var nextPlayer = gameTable.NextMovePlayer;
        var currentPlayer = gameTable.CurrentMovePlayer;

        if(!currentPlayer.IsBot || currentPlayer.BotDificulty == BotDificulty.ItIsNotABot)
        {
            throw new Exception($"Error! Bot can't move instead of player {currentPlayer.Name}");
        }
    }
}
