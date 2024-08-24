using BelieveOrNotBelieveGameServer.Models.BotModels;
using BelieveOrNotBelieveGameServer.Models;

namespace BelieveOrNotBelieveGameServer.Services.Abstraction;

public interface IBotNotFirstMoveService
{
    BotResponse MakeNotFirstMove(
        Player bot, 
        List<Player> otherPlayers, 
        List<PlayingCard> cardForDiscard, 
        GameTable gameTable);
}
