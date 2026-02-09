using GameCore.Common;
using GameCore.Models;
using GameCore.Sessions;

namespace GameCore.Abstractions;

public interface IGameSessionService
{
    Result CreateSession(string name, int numOfCards, int maxPlayers, bool addBot);
    bool SessionExistsByName(string name);
    bool SessionExistsByPlayerConnectionId(string playerConnectionId);
    Result DeleteSessionByName(string name);
    
    Result JoinSession(string playerConnectionId, string userName, string sessionName);
    Result LeaveSession(string playerConnectionId);

    Result MakeMove(string playerConnectionId, Move move);
    Result MakeAssumption(string playerConnectionId, bool isBelieving);
}