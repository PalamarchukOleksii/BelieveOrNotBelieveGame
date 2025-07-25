using Domain.Dtos;
using Domain.Enums;
using Domain.Models.GameModels;

namespace Presentation.Domain;

public class GameEngine
{
    private readonly List<Player> _players;
    private int _position;

    public Player CurrentMovePlayer { get; private set; } = new();
    public Player PreviousMovePlayer { get; private set; } = new();
    public Player NextMovePlayer { get; private set; } = new();
    
    public List<PlayingCard> CardsOnTable { get; private set; } = [];
    public List<PlayingCard> CardsForDiscard { get; private set; } = [];
    public int CountCardsForDiscard { get; private set; } = 0;
    public Move? CurrentMove { get; private set; }
    public List<Player> PlayersWhoWin { get; private set; } = [];
    
    public GameEngine(List<Player> players)
    {
        _players = players;
        InitPlayersOrder();
    }

    public void Reset()
    {
        _position = CountCardsForDiscard = 0;
        CurrentMove = null;
        CardsOnTable = [];

        _players.AddRange(PlayersWhoWin);

        PlayersWhoWin = [];

        foreach (var player in _players)
        {
            player.OnGameEnd();
        }
    }

    private void InitPlayersOrder()
    {
        Random rnd = new();
        _position = rnd.Next(0, _players.Count);
        CurrentMovePlayer = _players[_position];
        NextMovePlayer = _players[(_position + 1) % _players.Count];
    }
    
    public MoveCheckResult IsMoveAllowedForPlayer(Player player, IReadOnlyList<int> cardsId)
    {
        if (player.PlayersCards.Count == 0)
            return MoveCheckResult.ZeroCards;

        if (player.Name != CurrentMovePlayer.Name)
            return MoveCheckResult.NotPlayersTurn;

        if (!player.CheckIfPlayerHaveSomeCards(cardsId))
            return MoveCheckResult.DoesNotHaveCards;

        return MoveCheckResult.CanMakeMove;
    }
    
    public bool IsAssumeAllowedForPlayer(Player player) => player.Name == CurrentMovePlayer.Name;
    
    public void MakeMove(Move move)
    {
        CurrentMove = move;
        var player = _players.Single(x => x.Name == move.PlayerName);
        var cardsToRemove = new List<PlayingCard>();

        foreach (var card in player.PlayersCards)
        {
            foreach (var id in move.CardsId)
            {
                if (card.Id == id)
                {
                    CardsOnTable.Add(card);
                    cardsToRemove.Add(card);
                }
            }
        }

        foreach (var card in cardsToRemove)
        {
            player.PlayersCards.Remove(card);
        }

        AdvanceTurn();
    }
    
    public (bool EndGame, string Message) MakeAssume(bool isBelieved)
        {
            bool allCardsIsCorrect = true;
            (bool EndGame, string Message) result;

            for (int i = CardsOnTable.Count - 1; i >= CardsOnTable.Count - CurrentMove?.CardsId.Count; i--)
            {
                if (CardsOnTable[i].Value != CurrentMove?.CardValue)
                {
                    allCardsIsCorrect = false;
                    break;
                }
            }

            if (allCardsIsCorrect && isBelieved)
            {
                CountCardsForDiscard = CardsOnTable.Count;
                CardsForDiscard.AddRange(CardsOnTable);

                if (_players.TrueForAll(x => !x.PlayersCards.Any()))
                {
                    result = (true, $"Game over, {CurrentMovePlayer} lose, player {PreviousMovePlayer} do not lie");
                }
                else if (_players.Count(x => x.PlayersCards.Count > 0) == 1)
                {
                    result = (true, $"Game over, {_players.Single(x => x.PlayersCards.Count > 0).Name} lose, player {PreviousMovePlayer} do not lie");
                }
                else if (_players.Single(x => x.Name == CurrentMovePlayer.Name).PlayersCards.Count > 0)
                {
                    List<Player> plWithNoCards = _players.Where(x => x.PlayersCards.Count == 0).ToList();
                    PlayersWhoWin.AddRange(plWithNoCards);

                    _players.RemoveAll(x => x.PlayersCards.Count == 0);
                    NextMovePlayer = _players[(_players.IndexOf(_players.Single(x => x.Name == CurrentMovePlayer.Name)) + 1) % _players.Count];

                    result = (false, $"{CurrentMovePlayer} player make next move, player {PreviousMovePlayer} do not lie");
                }
                else
                {
                    AdvanceTurnWhenDelete();
                    result = (false, $"{CurrentMovePlayer} player make next move, player {PreviousMovePlayer} do not lie");
                }
            }
            else if (allCardsIsCorrect && !isBelieved)
            {
                _players.Single(x => x.Name == CurrentMovePlayer.Name).PlayersCards.AddRange(CardsOnTable);

                if (_players.Count(x => x.PlayersCards.Count > 0) == 1)
                {
                    result = (true, $"Game over, {_players.Single(x => x.PlayersCards.Count > 0).Name} lose, player {PreviousMovePlayer} do not lie");
                }
                else if (_players.Single(x => x.Name == NextMovePlayer.Name).PlayersCards.Count > 0)
                {
                    List<Player> plWithNoCards = _players.Where(x => x.PlayersCards.Count == 0).ToList();
                    PlayersWhoWin.AddRange(plWithNoCards);
                    _players.RemoveAll(x => x.PlayersCards.Count == 0);

                    result = (false, $"{NextMovePlayer} player make next move, player {PreviousMovePlayer} do not lie");
                    AdvanceTurn();
                }
                else
                {
                    AdvanceTurnWhenDelete();
                    result = (false, $"{CurrentMovePlayer} player make next move, player {PreviousMovePlayer} do not lie");
                }
            }
            else if (!allCardsIsCorrect && isBelieved)
            {
                _players.Single(x => x.Name == CurrentMovePlayer.Name).PlayersCards.AddRange(CardsOnTable);

                if (_players.Count(x => x.PlayersCards.Count > 0) == 1)
                {
                    result = new(true, $"Game over, {_players.Single(x => x.PlayersCards.Count > 0).Name} lose, player {PreviousMovePlayer} do lie");
                }
                else if (_players.Single(x => x.Name == NextMovePlayer.Name).PlayersCards.Count > 0)
                {
                    List<Player> plWithNoCards = _players.Where(x => x.PlayersCards.Count == 0).ToList();
                    PlayersWhoWin.AddRange(plWithNoCards);
                    _players.RemoveAll(x => x.PlayersCards.Count == 0);

                    result = new(false, $"{NextMovePlayer} player make next move, player {PreviousMovePlayer} dot lie");
                    AdvanceTurn();
                }
                else
                {
                    AdvanceTurnWhenDelete();
                    result = new(false, $"{CurrentMovePlayer} player make next move, player {PreviousMovePlayer} do lie");
                }
            }
            else
            {
                _players.Single(x => x.Name == PreviousMovePlayer.Name).PlayersCards.AddRange(CardsOnTable);

                if (_players.Count(x => x.PlayersCards.Count > 0) == 1)
                {
                    result = (true, $"Game over, {_players.Single(x => x.PlayersCards.Count > 0).Name} lose, player {PreviousMovePlayer} do lie");
                }
                else if (_players.Single(x => x.Name == CurrentMovePlayer.Name).PlayersCards.Count > 0)
                {
                    List<Player> plWithNoCards = _players.Where(x => x.PlayersCards.Count == 0).ToList();
                    PlayersWhoWin.AddRange(plWithNoCards);

                    _players.RemoveAll(x => x.PlayersCards.Count == 0);
                    NextMovePlayer = _players[(_players.IndexOf(_players.Single(x => x.Name == CurrentMovePlayer.Name)) + 1) % _players.Count];

                    result = (false, $"{CurrentMovePlayer} player make next move, player {PreviousMovePlayer} do lie");
                }
                else
                {
                    AdvanceTurnWhenDelete();
                    result = (false, $"{CurrentMovePlayer} player make next move, player {PreviousMovePlayer} do lie");
                }
            }

            CardsOnTable.Clear();
            return result;
        }
    
    private void AdvanceTurn()
    {
        PreviousMovePlayer = CurrentMovePlayer;
        CurrentMovePlayer = NextMovePlayer;

        _position = _players.IndexOf(CurrentMovePlayer);
        NextMovePlayer = _players[(_position + 1) % _players.Count];
    }
    
    private void AdvanceTurnWhenDelete()
    {
        List<Player> plWithNoCards = _players.Where(x => x.PlayersCards.Count == 0).ToList();
        PlayersWhoWin.AddRange(plWithNoCards);

        int currentPlIndex = _players.IndexOf(_players.Single(x => x.Name == CurrentMovePlayer.Name));
        int nextPlIndex = 0;

        for (int i = currentPlIndex + 1 % _players.Count, c = 0; c < _players.Count; c++, i = (i + 1) % _players.Count)
        {
            if (_players[i].PlayersCards.Count != 0)
            {
                nextPlIndex = i;
                break;
            }
        }

        CurrentMovePlayer = _players[nextPlIndex];
        _players.RemoveAll(x => x.PlayersCards.Count == 0);

        NextMovePlayer = _players[(_players.IndexOf(_players.Single(x => x.Name == CurrentMovePlayer.Name)) + 1) % _players.Count];
        _position = _players.IndexOf(_players.Single(x => x.Name == CurrentMovePlayer.Name));
    }
}