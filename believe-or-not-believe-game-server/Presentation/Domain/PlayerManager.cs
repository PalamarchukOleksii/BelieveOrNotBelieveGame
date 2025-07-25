using Domain.Dtos;
using Domain.Models.GameModels;

namespace Presentation.Domain;

public class PlayerManager
{
    private readonly List<Player> _players = [];
    private readonly int _maxPlayersCount;

    public PlayerManager(int maxPlayersCount)
    {
        _maxPlayersCount = maxPlayersCount;
    }
    
    public IReadOnlyList<Player> Players => _players;
    
    public bool AddPlayer(string username, string connectionId)
    {
        if (_players.Exists(x => x.Name == username) || _players.Count >= _maxPlayersCount) return false;
        _players.Add(new Player(connectionId, username));
        return true;
    }
    
    public bool RemovePlayer(string connectionId)
    {
        var player = _players.Find(x => x.PlayerConnectionId == connectionId);
        if (player == null) return false;
            
        _players.Remove(player);
        return true;
    }
    
    public Player? GetPlayerByName(string name) => _players.Find(x => x.Name == name);
    
    public Player? GetPlayerByConnectionId(string connectionId) => _players.Find(x => x.PlayerConnectionId == connectionId);
    
    public List<ShortOpponentInfoDto> GetShortInfoAboutPlayers()
    {
        return _players.Select(x => new ShortOpponentInfoDto
        {
            PlayerConnectionId = x.PlayerConnectionId,
            Name = x.Name,
            CardCount = x.PlayersCards.Count
        }).ToList();
    }
    
    public List<PlayerCardsDto> GetPlayersCards()
    {
        return _players.Select(x => new PlayerCardsDto
        {
            PlayerConnectionId = x.PlayerConnectionId,
            Cards = x.PlayersCards,
        }).ToList();
    }

    public void ApplyPlayerStartGame(string connectionId)
    {
        var player = GetPlayerByConnectionId(connectionId);
        player?.SetStartGame(true);
    }
    
    public bool AreAllPlayersReady() => _players.Count > 0 && _players.All(p => p.StartGame);
}