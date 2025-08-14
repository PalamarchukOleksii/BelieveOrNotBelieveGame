using GameCore.Common;
using GameCore.Dtos;
using GameCore.Enums;
using GameCore.Models;

namespace GameCore.Managers;

public class PlayerManager
{
    private readonly int _maxPlayersCount;
    private readonly List<Player> _players = [];

    public PlayerManager(int maxPlayersCount)
    {
        _maxPlayersCount = maxPlayersCount;
    }

    public IReadOnlyList<Player> Players => _players;

    public Result Add(string username, string connectionId)
    {
        if (_players.Any(x => x.Name == username))
        {
            return Result.Failure(
                code: "Player.DuplicateName",
                message: $"A player with the name '{username}' already exists.",
                type: ErrorType.Client
            );
        }

        if (_players.Count >= _maxPlayersCount)
        {
            return Result.Failure(
                code: "Player.LimitReached",
                message: "The maximum number of players has been reached.",
                type: ErrorType.Client
            );
        }

        _players.Add(new Player(connectionId, username));
        return Result.Success();
    }

    public Result Remove(string connectionId)
    {
        var player = _players.Find(x => x.ConnectionId == connectionId);
        if (player is null)
        {
            return Result.Failure(
                code: "Player.NotFound",
                message: $"No player found with connection ID '{connectionId}'.",
                type: ErrorType.Client
            );
        }

        _players.Remove(player);
        return Result.Success();
    }
    
    public Result<Player> GetByName(string name)
    {
        var player = _players.Find(x => x.Name == name);
        return player is not null
            ? Result<Player>.Success(player)
            : Result<Player>.Failure(
                code: "Player.NotFound",
                message: $"No player found with name '{name}'.",
                type: ErrorType.Client
            );
    }

    public Result<Player> GetByConnectionId(string connectionId)
    {
        var player = _players.Find(x => x.ConnectionId == connectionId);
        return player is not null
            ? Result<Player>.Success(player)
            : Result<Player>.Failure(
                code: "Player.NotFound",
                message: $"No player found with connection ID '{connectionId}'.",
                type: ErrorType.Client
            );
    }

    public List<ShortOpponentInfoDto> GetShortInfoAboutPlayers()
    {
        return _players.Select(x => new ShortOpponentInfoDto
        {
            PlayerConnectionId = x.ConnectionId,
            Name = x.Name,
            CardCount = x.Cards.Count
        }).ToList();
    }

    public List<PlayerCardsDto> GetPlayersCards()
    {
        return _players.Select(x => new PlayerCardsDto
        {
            PlayerConnectionId = x.ConnectionId,
            Cards = x.Cards.ToList()
        }).ToList();
    }

    public Result ApplyStartGame(string connectionId)
    {
        var player = _players.Find(x => x.ConnectionId == connectionId);
        if (player is null)
        {
            return Result.Failure(
                code: "Player.NotFound",
                message: $"No player found with connection ID '{connectionId}'.",
                type: ErrorType.Client
            );
        }

        player.StartGame();
        return Result.Success();
    }

    public bool AreAllPlayersReady()
    {
        return _players.Count > 0 && _players.All(p => p.IsReadyToStart);
    }
}