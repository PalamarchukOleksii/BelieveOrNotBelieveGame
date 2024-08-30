using Domain.Models.BotModels;
using Domain.Models.GameModels;

namespace Domain.Abstractions.BotAbstractions;

public interface IBotService
{
    BotResponse MakeMove(GameTable gameTable);
}
