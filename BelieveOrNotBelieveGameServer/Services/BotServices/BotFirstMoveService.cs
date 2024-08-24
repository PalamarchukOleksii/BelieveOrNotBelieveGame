using BelieveOrNotBelieveGameServer.Models;
using BelieveOrNotBelieveGameServer.Models.BotModels;
using BelieveOrNotBelieveGameServer.Services.Abstraction;

namespace BelieveOrNotBelieveGameServer.Services.BotServices;

public class BotFirstMoveService : IBotFirstMoveService
{
    public BotResponse MakeFirstMove(Player bot, List<Player> otherPlayers, List<PlayingCard> cardForDiscard, GameTable gameTable)
    {
        throw new NotImplementedException();
        //return new BotResponse();
    }
}
