using BelieveOrNotBelieveGameServer.Models;
using BelieveOrNotBelieveGameServer.Models.BotModels;

namespace BelieveOrNotBelieveGameServer.Services.Abstraction;

public interface IBotService
{
    BotResponse MakeMove(GameTable gameTable);
}
