using BelieveOrNotBelieveGameServer.Models.BotModels;
using BelieveOrNotBelieveGameServer.Models;

namespace BelieveOrNotBelieveGameServer.Services.Abstraction;

public interface IBotMakeMoveService
{
    BotResponse MakeMove(BotInfo botInfo, bool isFirstMove);
}
