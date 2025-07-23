using Domain.Abstractions.BotAbstractions;
using Domain.Models.BotModels;

namespace Domain.Services.BotServices;

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
