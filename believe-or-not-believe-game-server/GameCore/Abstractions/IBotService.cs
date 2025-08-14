using GameCore.Models;
using GameCore.Sessions;

namespace GameCore.Abstractions;

public interface IBotService
{
    BotResponse MakeMove(GameSession gameTable);
}