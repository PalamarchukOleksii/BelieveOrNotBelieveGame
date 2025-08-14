using GameCore.Abstractions;
using GameCore.Models;

namespace GameCore.Services;

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