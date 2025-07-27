using System.Collections.Concurrent;
using GameCore.Abstractions.GameAbstractions;
using GameCore.Sessions;

namespace GameCore.Services.GameServices;

public class GameTableService : IGameTableService
{
    private readonly ConcurrentDictionary<string, GameSession> _connectionIdToTable = new();
    private readonly ConcurrentDictionary<string, GameSession> _gameTables = new();

    public GameSession? CreateGameSession(string gameName, int numOfCards, int maxNumOfPlayers, bool addBot)
    {
        var newGame = new GameSession(gameName, numOfCards, maxNumOfPlayers, addBot);
        var added = _gameTables.TryAdd(gameName, newGame);
        return added ? newGame : null;
    }

    public bool DeleteGameSessionByName(string gameName)
    {
        return _gameTables.TryRemove(gameName, out _);
    }

    public GameSession? GetGameSessionByName(string gameName)
    {
        _gameTables.TryGetValue(gameName, out var table);
        return table;
    }

    public GameSession? GetGameSessionByConnectionId(string connectionId)
    {
        _connectionIdToTable.TryGetValue(connectionId, out var table);
        return table;
    }

    public bool PlayerJoinGameSession(string username, string connectionId, string gameName)
    {
        if (!_gameTables.TryGetValue(gameName, out var table))
            return false;

        var joinResult = table.JoinGameTable(username, connectionId);
        if (joinResult) _connectionIdToTable.TryAdd(connectionId, table);

        return joinResult;
    }

    public bool PlayerLeaveGameSession(string connectionId)
    {
        if (!_connectionIdToTable.TryGetValue(connectionId, out var table))
            return false;

        var leaveResult = table.LeaveGameTable(connectionId);
        if (leaveResult) _connectionIdToTable.TryRemove(connectionId, out _);

        return leaveResult;
    }
}