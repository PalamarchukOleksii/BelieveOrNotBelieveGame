using Domain.Models.BotModels;

namespace Domain.Abstractions.BotAbstractions;

public interface IBotMakeMoveService
{
    BotResponse MakeMove(BotInfo botInfo, bool isFirstMove);
}
