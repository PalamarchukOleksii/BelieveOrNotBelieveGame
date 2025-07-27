using GameCore.Models.BotModels;
using GameCore.Sessions;

namespace GameCore.Abstractions.BotAbstractions;

public interface IBotService
{
    BotResponse MakeMove(GameSession gameTable);
}