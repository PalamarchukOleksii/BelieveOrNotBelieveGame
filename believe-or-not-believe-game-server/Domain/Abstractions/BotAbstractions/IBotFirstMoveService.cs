using Domain.Models.BotModels;

namespace Domain.Abstractions.BotAbstractions;

public interface IBotFirstMoveService
{
    BotResponse MakeFirstMove(BotInfo botInfo);
}
