using GameCore.Models;

namespace GameCore.Abstractions;

public interface IBotMakeMoveService
{
    BotResponse MakeMove(BotInfo botInfo, bool isFirstMove);
}