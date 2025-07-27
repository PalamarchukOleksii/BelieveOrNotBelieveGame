using GameCore.Sessions;

namespace GameCore.Abstractions.GameAbstractions;

public interface IGameTableService
{
    GameSession? CreateGameSession(string gameName, int numOfCards, int maxNumOfPlayers, bool addBot);
    GameSession? GetGameSessionByName(string gameName);
    bool DeleteGameSessionByName(string gameName);
    GameSession? GetGameSessionByConnectionId(string connectionId);
    bool PlayerJoinGameSession(string username, string connectionId, string gameName);
    bool PlayerLeaveGameSession(string connectionId);
}