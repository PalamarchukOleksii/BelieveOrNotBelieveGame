using GameCore.Models.BotModels;

namespace GameCore.Abstractions.BotAbstractions;

public interface IBotMakeMoveService
{
    BotResponse MakeMove(BotInfo botInfo, bool isFirstMove);
}