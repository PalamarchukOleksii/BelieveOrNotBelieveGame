using Domain.Dtos;
using Domain.Enums;
using Domain.Models.GameModels;

namespace Presentation.Domain;

public class GameTable
{
    public string Name { get; init; }
    public int NumOfCards { get; init; } 
    public int MaxNumOfPlayers { get; init; }
    public bool AddBot { get; init; }
    private bool GameStarted { get; set; }

    private readonly PlayerManager _playerManager;
    private readonly DeckManager _deckManager;
    private GameEngine? _gameEngine;

    public GameTable(string gameName, int numOfCards, int maxNumOfPlayers, bool addBot)
    {
        Name = gameName;
        NumOfCards = numOfCards;
        MaxNumOfPlayers = maxNumOfPlayers;
        AddBot = addBot;

        _playerManager = new PlayerManager(maxNumOfPlayers);
        _deckManager = new DeckManager(numOfCards);
    }
    
    public bool JoinGameTable(string username, string connectionId) => _playerManager.AddPlayer(username, connectionId);

    public bool LeaveGameTable(string connectionId) => _playerManager.RemovePlayer(connectionId);
    
    public bool StartGameTable(string connectionId)
    {
        _playerManager.RemovePlayer(connectionId);

        if (!_playerManager.AreAllPlayersReady()) return false;
        
        GameStarted = true;

        _deckManager.InitializeDeck();
        _deckManager.DealCardsToPlayers(_playerManager.Players.ToList());
        
        _gameEngine = new GameEngine(_playerManager.Players.ToList());

        return GameStarted;

    }
    
    public void EndGameTable()
    {
        GameStarted = false;
        _gameEngine?.Reset();
    }
    
    public GameStateDto GetGameTableState()
    {
        if (_gameEngine == null)
        {
            return new GameStateDto();
        }
        
        return new GameStateDto
        {
            CurrentMovePlayerName = _gameEngine.CurrentMovePlayer.Name,
            CurrentMovePlayerConnectionId = _gameEngine.CurrentMovePlayer.PlayerConnectionId,
            CurrentPlayerCanMakeMove = _gameEngine.CurrentMovePlayer.PlayersCards.Count > 0,
            CurrentPlayerCanMakeAssume = _gameEngine.CardsOnTable.Count > 0,
            MakeMoveValue = _deckManager.CardsDeck.Values.ToList(),
            CardsOnTableCount = _gameEngine.CardsOnTable.Count,
        };
    }

    public MoveCheckResult CheckIfPlayerCanMakeMove(Player player, IReadOnlyList<int> cardsId) => _gameEngine?.IsMoveAllowedForPlayer(player, cardsId) ?? MoveCheckResult.NotPlayersTurn;

    public void MakeMove(Move move) => _gameEngine?.MakeMove(move);
    
    public void MakeAssume(bool isBelieved) => _gameEngine?.MakeAssume(isBelieved);
}