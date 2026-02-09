using GameCore.Models;

namespace GameCore.Abstractions;

public interface IBotNotFirstMoveService
{
    BotResponse MakeNotFirstMove(BotInfo botInfo);
}