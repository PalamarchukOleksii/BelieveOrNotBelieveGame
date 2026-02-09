using GameCore.Models;

namespace GameCore.Abstractions;

public interface IBotFirstMoveService
{
    BotResponse MakeFirstMove(BotInfo botInfo);
}