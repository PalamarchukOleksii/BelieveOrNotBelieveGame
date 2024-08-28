using BelieveOrNotBelieveGameServer.Models.BotModels;

namespace BelieveOrNotBelieveGameServer.Services.Abstraction;

public interface IBotFirstMoveService
{
    BotResponse MakeFirstMove(BotInfo botInfo);
}
