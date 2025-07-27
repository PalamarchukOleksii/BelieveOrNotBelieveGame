using GameCore.Enums;
using GameCore.Models.GameModels;

namespace GameCore.Engines;

public class GameEngine
{
    private readonly List<Player> _players;
    private int _position;

    public GameEngine(List<Player> players)
    {
        _players = players;
        InitPlayersOrder();
    }

    public Player CurrentMovePlayer { get; private set; } = new();
    public Player PreviousMovePlayer { get; private set; } = new();
    public Player NextMovePlayer { get; private set; } = new();

    public List<Card> CardsOnTable { get; private set; } = [];
    public List<Card> CardsForDiscard { get; } = [];
    public int CountCardsForDiscard { get; private set; }
    public Move? CurrentMove { get; private set; }
    public List<Player> PlayersWhoWin { get; private set; } = [];

    public void Reset()
    {
        _position = CountCardsForDiscard = 0;
        CurrentMove = null;
        CardsOnTable = [];

        _players.AddRange(PlayersWhoWin);

        PlayersWhoWin = [];

        foreach (var player in _players) player.OnGameEnd();
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

    public bool IsAssumeAllowedForPlayer(Player player)
    {
        return player.Name == CurrentMovePlayer.Name;
    }

    public void MakeMove(Move move)
    {
        CurrentMove = move;
        var player = _players.Single(x => x.Name == move.PlayerName);
        var cardsToRemove = new List<Card>();

        foreach (var card in player.PlayersCards)
        foreach (var id in move.CardsId)
            if (card.Id == id)
            {
                CardsOnTable.Add(card);
                cardsToRemove.Add(card);
            }

        foreach (var card in cardsToRemove) player.PlayersCards.Remove(card);

        AdvanceTurn();
    }

    public (bool EndGame, string Message) MakeAssume(bool isBelieved)
    {
        var allCardsIsCorrect = true;
        (bool EndGame, string Message) result;

        for (var i = CardsOnTable.Count - 1; i >= CardsOnTable.Count - CurrentMove?.CardsId.Count; i--)
            if (CardsOnTable[i].Value != CurrentMove?.CardValue)
            {
                allCardsIsCorrect = false;
                break;
            }

        if (allCardsIsCorrect && isBelieved)
        {
            CountCardsForDiscard = CardsOnTable.Count;
            CardsForDiscard.AddRange(CardsOnTable);

            if (_players.TrueForAll(x => !x.PlayersCards.Any()))
            {
                result = (true, $"GameModels over, {CurrentMovePlayer} lose, player {PreviousMovePlayer} do not lie");
            }
            else if (_players.Count(x => x.PlayersCards.Count > 0) == 1)
            {
                result = (true,
                    $"GameModels over, {_players.Single(x => x.PlayersCards.Count > 0).Name} lose, player {PreviousMovePlayer} do not lie");
            }
            else if (_players.Single(x => x.Name == CurrentMovePlayer.Name).PlayersCards.Count > 0)
            {
                var plWithNoCards = _players.Where(x => x.PlayersCards.Count == 0).ToList();
                PlayersWhoWin.AddRange(plWithNoCards);

                _players.RemoveAll(x => x.PlayersCards.Count == 0);
                NextMovePlayer =
                    _players[
                        (_players.IndexOf(_players.Single(x => x.Name == CurrentMovePlayer.Name)) + 1) %
                        _players.Count];

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
                result = (true,
                    $"GameModels over, {_players.Single(x => x.PlayersCards.Count > 0).Name} lose, player {PreviousMovePlayer} do not lie");
            }
            else if (_players.Single(x => x.Name == NextMovePlayer.Name).PlayersCards.Count > 0)
            {
                var plWithNoCards = _players.Where(x => x.PlayersCards.Count == 0).ToList();
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
                result = new ValueTuple<bool, string>(true,
                    $"GameModels over, {_players.Single(x => x.PlayersCards.Count > 0).Name} lose, player {PreviousMovePlayer} do lie");
            }
            else if (_players.Single(x => x.Name == NextMovePlayer.Name).PlayersCards.Count > 0)
            {
                var plWithNoCards = _players.Where(x => x.PlayersCards.Count == 0).ToList();
                PlayersWhoWin.AddRange(plWithNoCards);
                _players.RemoveAll(x => x.PlayersCards.Count == 0);

                result = new ValueTuple<bool, string>(false,
                    $"{NextMovePlayer} player make next move, player {PreviousMovePlayer} dot lie");
                AdvanceTurn();
            }
            else
            {
                AdvanceTurnWhenDelete();
                result = new ValueTuple<bool, string>(false,
                    $"{CurrentMovePlayer} player make next move, player {PreviousMovePlayer} do lie");
            }
        }
        else
        {
            _players.Single(x => x.Name == PreviousMovePlayer.Name).PlayersCards.AddRange(CardsOnTable);

            if (_players.Count(x => x.PlayersCards.Count > 0) == 1)
            {
                result = (true,
                    $"GameModels over, {_players.Single(x => x.PlayersCards.Count > 0).Name} lose, player {PreviousMovePlayer} do lie");
            }
            else if (_players.Single(x => x.Name == CurrentMovePlayer.Name).PlayersCards.Count > 0)
            {
                var plWithNoCards = _players.Where(x => x.PlayersCards.Count == 0).ToList();
                PlayersWhoWin.AddRange(plWithNoCards);

                _players.RemoveAll(x => x.PlayersCards.Count == 0);
                NextMovePlayer =
                    _players[
                        (_players.IndexOf(_players.Single(x => x.Name == CurrentMovePlayer.Name)) + 1) %
                        _players.Count];

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
        var plWithNoCards = _players.Where(x => x.PlayersCards.Count == 0).ToList();
        PlayersWhoWin.AddRange(plWithNoCards);

        var currentPlIndex = _players.IndexOf(_players.Single(x => x.Name == CurrentMovePlayer.Name));
        var nextPlIndex = 0;

        for (int i = currentPlIndex + 1 % _players.Count, c = 0; c < _players.Count; c++, i = (i + 1) % _players.Count)
            if (_players[i].PlayersCards.Count != 0)
            {
                nextPlIndex = i;
                break;
            }

        CurrentMovePlayer = _players[nextPlIndex];
        _players.RemoveAll(x => x.PlayersCards.Count == 0);

        NextMovePlayer =
            _players[(_players.IndexOf(_players.Single(x => x.Name == CurrentMovePlayer.Name)) + 1) % _players.Count];
        _position = _players.IndexOf(_players.Single(x => x.Name == CurrentMovePlayer.Name));
    }
}