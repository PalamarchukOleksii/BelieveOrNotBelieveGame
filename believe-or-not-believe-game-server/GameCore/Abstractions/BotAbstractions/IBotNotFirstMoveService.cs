using GameCore.Models.BotModels;

namespace GameCore.Abstractions.BotAbstractions;

public interface IBotNotFirstMoveService
{
    BotResponse MakeNotFirstMove(BotInfo botInfo);
}