using Domain.Models.BotModels;

namespace Domain.Abstractions.BotAbstractions;

public interface IBotNotFirstMoveService
{
    BotResponse MakeNotFirstMove(BotInfo botInfo);
}
