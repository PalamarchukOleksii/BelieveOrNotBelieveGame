using GameCore.Models.BotModels;

namespace GameCore.Abstractions.BotAbstractions;

public interface IBotFirstMoveService
{
    BotResponse MakeFirstMove(BotInfo botInfo);
}