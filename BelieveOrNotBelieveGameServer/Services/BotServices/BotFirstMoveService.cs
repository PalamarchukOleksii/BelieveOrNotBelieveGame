using BelieveOrNotBelieveGameServer.Models.BotModels;
using BelieveOrNotBelieveGameServer.Services.Abstraction;

namespace BelieveOrNotBelieveGameServer.Services.BotServices;

public class BotFirstMoveService : IBotFirstMoveService
{
    private readonly IBotMakeMoveService _botMakeMoveService;

    public BotFirstMoveService(IBotMakeMoveService botMakeMoveService)
    {
        _botMakeMoveService = botMakeMoveService;
    }

    public BotResponse MakeFirstMove(BotInfo botInfo)
    {
        return _botMakeMoveService.MakeMove(botInfo, true);
    }
}
