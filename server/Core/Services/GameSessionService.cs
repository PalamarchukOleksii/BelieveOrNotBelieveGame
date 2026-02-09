using System.Collections.Concurrent;
using GameCore.Abstractions;
using GameCore.Common;
using GameCore.Enums;
using GameCore.Models;
using GameCore.Sessions;

namespace GameCore.Services;

public class GameSessionService : IGameSessionService
{
    private readonly ConcurrentDictionary<string, GameSession> _gameSessionsByName = new();
    private readonly ConcurrentDictionary<string, GameSession> _gameSessionsByPlayerConnectionId = new();

    public Result CreateSession(string name, int numOfCards, int maxPlayers, bool addBot)
    {
        var newGame = new GameSession(name, numOfCards, maxPlayers, addBot);
        var added = _gameSessionsByName.TryAdd(name, newGame);

        return added
            ? Result.Success()
            : Result.Failure(
                code: "GameSession.AlreadyExists",
                message: $"A game session with the name '{name}' already exists.",
                type: ErrorType.Client
            );
    }

    public bool SessionExistsByPlayerConnectionId(string playerConnectionId)
    {
        return _gameSessionsByPlayerConnectionId.ContainsKey(playerConnectionId);
    }

    public bool SessionExistsByName(string name)
    {
        return _gameSessionsByName.ContainsKey(name);
    }

    public Result DeleteSessionByName(string name)
    {
        var removed = _gameSessionsByName.TryRemove(name, out _);

        return removed
            ? Result.Success()
            : Result.Failure(
                code: "GameSession.NotFound",
                message: $"No game session found with the name '{name}'.",
                type: ErrorType.Client
            );
    }

    public Result JoinSession(string playerConnectionId, string userName, string sessionName)
    {
        if (!_gameSessionsByName.TryGetValue(sessionName, out var session))
            return Result.Failure(
                code: "GameSession.NotFound",
                message: $"No game session found with the name '{sessionName}'.",
                type: ErrorType.Client
            );

        var joinResult = session.Join(userName, playerConnectionId);
        if (!joinResult.IsSuccess)
            return joinResult;
        
        _gameSessionsByPlayerConnectionId[playerConnectionId] = session;

        return Result.Success();
    }

    public Result LeaveSession(string playerConnectionId)
    {
        if (!_gameSessionsByPlayerConnectionId.TryGetValue(playerConnectionId, out var session))
        {
            return Result.Failure(
                code: "GameSession.NotFound",
                message: $"No game session found for player connection ID '{playerConnectionId}'.",
                type: ErrorType.Client
            );
        }

        var leaveResult = session.Leave(playerConnectionId);
        if (!leaveResult.IsSuccess)
            return leaveResult;
        
        _gameSessionsByPlayerConnectionId.TryRemove(playerConnectionId, out _);

        return Result.Success();
    }

    public Result MakeMove(string playerConnectionId, Move move)
    {
        throw new NotImplementedException();
    }

    public Result MakeAssumption(string playerConnectionId, bool isBelieving)
    {
        throw new NotImplementedException();
    }
}
