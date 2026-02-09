using GameCore.Common;
using GameCore.Dtos;
using GameCore.Enums;
using GameCore.Managers;
using GameCore.Models;

namespace GameCore.Sessions;

public class GameSession
{
    private readonly DeckManager _deckManager;
    private readonly PlayerManager _playerManager;
    private TurnManager? _turnManager;

    public GameSession(string gameName, int numOfCards, int maxNumOfPlayers, bool addBot)
    {
        Name = gameName;
        NumOfCards = numOfCards;
        MaxNumOfPlayers = maxNumOfPlayers;
        AddBot = addBot;

        _playerManager = new PlayerManager(maxNumOfPlayers);
        _deckManager = new DeckManager(numOfCards);
    }

    public string Name { get; init; }
    public int NumOfCards { get; init; }
    public int MaxNumOfPlayers { get; init; }
    public bool AddBot { get; init; }
    private bool GameStarted { get; set; }

    public Result Join(string username, string connectionId)
    {
        return _playerManager.Add(username, connectionId);
    }

    public Result Leave(string connectionId)
    {
        return _playerManager.Remove(connectionId);
    }

    public Result Start(string connectionId)
    {
        _playerManager.ApplyStartGame(connectionId);

        if (!_playerManager.AreAllPlayersReady())
        {
            return Result.Failure(
                code: "Game.NotAllPlayersReady",
                message: "Not all players are ready to start the game.",
                type: ErrorType.Validation
            );
        }

        GameStarted = true;

        _deckManager.ShuffleDeck();
        _deckManager.DealCardsToPlayers(_playerManager.Players.ToList());

        _turnManager = new TurnManager(_playerManager.Players.ToList());

        return Result.Success();
    }


    public void End()
    {
        GameStarted = false;
        _turnManager?.Reset();
    }

    public GameStateDto GetState()
    {
        if (_turnManager == null) return new GameStateDto();

        return new GameStateDto
        {
            CurrentMovePlayerName = _turnManager.CurrentMovePlayer.Name,
            CurrentMovePlayerConnectionId = _turnManager.CurrentMovePlayer.ConnectionId,
            CurrentPlayerCanMakeMove = _turnManager.CurrentMovePlayer.Cards.Count > 0,
            CurrentPlayerCanMakeAssume = _turnManager.CardsOnTable.Count > 0,
            MakeMoveValue = _deckManager.DeckValues.ToList(),
            CardsOnTableCount = _turnManager.CardsOnTable.Count
        };
    }

    public Result MakeMove(string playerConnectionId, Move move)
    {
        var playerResult = _playerManager.GetByConnectionId(playerConnectionId);
        if (!playerResult.IsSuccess)
        {
            return playerResult;
        }

        if (_turnManager is null)
        {
            return Result.Failure(
                code: "Game.TurnManagerNotInitialized",
                message: "Turn manager is not initialized.",
                type: ErrorType.Server
            );
        }

        return _turnManager.MakeMove(playerResult.Value, move);
    }

    public void MakeAssumption(bool isBelieved)
    {
        _turnManager?.MakeAssumption(isBelieved);
    }
}