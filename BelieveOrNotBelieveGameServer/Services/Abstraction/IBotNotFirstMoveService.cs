using BelieveOrNotBelieveGameServer.Models.BotModels;

namespace BelieveOrNotBelieveGameServer.Services.Abstraction;

public interface IBotNotFirstMoveService
{
    BotResponse MakeNotFirstMove(BotInfo botInfo);
}
