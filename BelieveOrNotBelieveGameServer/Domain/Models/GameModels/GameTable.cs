using Domain.Common.Helpers;
using Domain.Common.Options;

namespace Domain.Models.GameModels
{
    public class GameTable
    {
        public GameTableOptions Options { get; set; } = new GameTableOptions();
        public List<Player> Players { get; set; } = new List<Player>();
        public List<PlayingCard> CardsOnTable { get; set; } = new List<PlayingCard>();
        public List<PlayingCard> CardsForDiscard { get; set; } = new List<PlayingCard>();
        public Move? Move { get; set; }
        public Player CurrentMovePlayer { get; set; } = new Player();
        public Player PreviousMovePlayer { get; set; } = new Player();
        public Player NextMovePlayer { get; set; } = new Player();
        private int Position = 0;
        public bool GameStarted { get; set; } = false;
        public int CountCardsForDiscard { get; set; } = 0;
        public List<Player> PlayersWhoWin { get; set; } = new List<Player>();

        public bool JoinGameTable(string username, string connectionId)
        {
            if (!Players.Exists(x => x.Name == username))
            {
                Players.Add(new Player { PlayerConnectionId = connectionId, Name = username });
                return true;
            }

            return false;
        }

        public Player? GetPlayerWithConnectionId(string connectionId)
        {
            return Players.Find(x => x.PlayerConnectionId == connectionId);
        }

        public void StartGame()
        {
            GameStarted = true;
            CardsDeck deck = new CardsDeck(Options.NumOfCards);
            CardsDeckHelper.ShuffleCards(deck);
            //deck.ShuffleCards();
            CardsDeckHelper.GiveCardsToPlayers(deck, Players);
            //deck.GiveCardsToPlayers(Players);

            Random rnd = new Random();
            Position = rnd.Next(0, Players.Count - 1);

            CurrentMovePlayer = Players[Position];
            if (Position == Players.Count - 1)
            {
                NextMovePlayer = Players[0];
            }
            else
            {
                NextMovePlayer = Players[Position + 1];
            }
        }

        public void MakeMove(Move move)
        {
            Move = move;
            var player = Players.Single(x => x.Name == move.PlayerName);
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

            CalcPlayer();
        }

        public (int ResultId, string ClientMessage) MakeAssume(bool iBelieve)
        {
            bool allCardsIsCorrect = true;
            (int ResultId, string ClientMessage) result;

            for (int i = CardsOnTable.Count - 1; i >= CardsOnTable.Count - Move?.CardsId.Count; i--)
            {
                if (CardsOnTable[i].Value != Move?.CardValue)
                {
                    allCardsIsCorrect = false;
                    break;
                }
            }

            if (allCardsIsCorrect && iBelieve)
            {
                CountCardsForDiscard = CardsOnTable.Count;
                CardsForDiscard.AddRange(CardsOnTable);

                if (Players.TrueForAll(x => !x.PlayersCards.Any()))
                {
                    result = (5, $"Game over, {CurrentMovePlayer} lose, player {PreviousMovePlayer} do not lie");
                }
                else if (Players.Count(x => x.PlayersCards.Count > 0) == 1)
                {
                    result = (5, $"Game over, {Players.Single(x => x.PlayersCards.Count > 0).Name} lose, player {PreviousMovePlayer} do not lie");
                }
                else if (Players.Single(x => x.Name == CurrentMovePlayer.Name).PlayersCards.Count > 0)
                {
                    List<Player> plWithNoCards = Players.Where(x => x.PlayersCards.Count == 0).ToList();
                    PlayersWhoWin.AddRange(plWithNoCards);

                    Players.RemoveAll(x => x.PlayersCards.Count == 0);
                    NextMovePlayer = Players[(Players.IndexOf(Players.Single(x => x.Name == CurrentMovePlayer.Name)) + 1) % Players.Count];

                    result = (1, $"{CurrentMovePlayer} player make next move, player {PreviousMovePlayer} do not lie");
                }
                else
                {
                    CalcPlayerWhenDelete();
                    result = (1, $"{CurrentMovePlayer} player make next move, player {PreviousMovePlayer} do not lie");
                }
            }
            else if (allCardsIsCorrect && !iBelieve)
            {
                Players.Single(x => x.Name == CurrentMovePlayer.Name).PlayersCards.AddRange(CardsOnTable);

                if (Players.Count(x => x.PlayersCards.Count > 0) == 1)
                {
                    result = (5, $"Game over, {Players.Single(x => x.PlayersCards.Count > 0).Name} lose, player {PreviousMovePlayer} do not lie");
                }
                else if (Players.Single(x => x.Name == NextMovePlayer.Name).PlayersCards.Count > 0)
                {
                    List<Player> plWithNoCards = Players.Where(x => x.PlayersCards.Count == 0).ToList();
                    PlayersWhoWin.AddRange(plWithNoCards);
                    Players.RemoveAll(x => x.PlayersCards.Count == 0);

                    result = (2, $"{NextMovePlayer} player make next move, player {PreviousMovePlayer} do not lie");
                    CalcPlayer();
                }
                else
                {
                    CalcPlayerWhenDelete();
                    result = (2, $"{CurrentMovePlayer} player make next move, player {PreviousMovePlayer} do not lie");
                }
            }
            else if (!allCardsIsCorrect && iBelieve)
            {
                Players.Single(x => x.Name == CurrentMovePlayer.Name).PlayersCards.AddRange(CardsOnTable);

                if (Players.Count(x => x.PlayersCards.Count > 0) == 1)
                {
                    result = new(5, $"Game over, {Players.Single(x => x.PlayersCards.Count > 0).Name} lose, player {PreviousMovePlayer} do lie");
                }
                else if (Players.Single(x => x.Name == NextMovePlayer.Name).PlayersCards.Count > 0)
                {
                    List<Player> plWithNoCards = Players.Where(x => x.PlayersCards.Count == 0).ToList();
                    PlayersWhoWin.AddRange(plWithNoCards);
                    Players.RemoveAll(x => x.PlayersCards.Count == 0);

                    result = new(3, $"{NextMovePlayer} player make next move, player {PreviousMovePlayer} dot lie");
                    CalcPlayer();
                }
                else
                {
                    CalcPlayerWhenDelete();
                    result = new(3, $"{CurrentMovePlayer} player make next move, player {PreviousMovePlayer} do lie");
                }
            }
            else
            {
                Players.Single(x => x.Name == PreviousMovePlayer.Name).PlayersCards.AddRange(CardsOnTable);

                if (Players.Count(x => x.PlayersCards.Count > 0) == 1)
                {
                    result = (5, $"Game over, {Players.Single(x => x.PlayersCards.Count > 0).Name} lose, player {PreviousMovePlayer} do lie");
                }
                else if (Players.Single(x => x.Name == CurrentMovePlayer.Name).PlayersCards.Count > 0)
                {
                    List<Player> plWithNoCards = Players.Where(x => x.PlayersCards.Count == 0).ToList();
                    PlayersWhoWin.AddRange(plWithNoCards);

                    Players.RemoveAll(x => x.PlayersCards.Count == 0);
                    NextMovePlayer = Players[(Players.IndexOf(Players.Single(x => x.Name == CurrentMovePlayer.Name)) + 1) % Players.Count];

                    result = (4, $"{CurrentMovePlayer} player make next move, player {PreviousMovePlayer} do lie");
                }
                else
                {
                    CalcPlayerWhenDelete();
                    result = (4, $"{CurrentMovePlayer} player make next move, player {PreviousMovePlayer} do lie");
                }
            }

            CardsOnTable.Clear();
            return result;
        }

        public void EndGame()
        {
            GameStarted = false;
            Position = CountCardsForDiscard = 0;
            Move = null;
            CardsOnTable = new List<PlayingCard>();

            Players.AddRange(PlayersWhoWin);

            PlayersWhoWin = new List<Player>();

            foreach (var player in Players)
            {
                player.StartGame = false;
                player.PlayersCards = new List<PlayingCard>();
            }
        }

        private void CalcPlayer()
        {
            PreviousMovePlayer = CurrentMovePlayer;
            CurrentMovePlayer = NextMovePlayer;

            Position = Players.IndexOf(CurrentMovePlayer);

            NextMovePlayer = Players[(Position + 1) % Players.Count];
        }

        private void CalcPlayerWhenDelete()
        {
            List<Player> plWithNoCards = Players.Where(x => x.PlayersCards.Count == 0).ToList();
            PlayersWhoWin.AddRange(plWithNoCards);

            int currentPlIndex = Players.IndexOf(Players.Single(x => x.Name == CurrentMovePlayer.Name));
            int nextPlIndex = 0;

            for (int i = currentPlIndex + 1 % Players.Count, c = 0; c < Players.Count; c++, i = (i + 1) % Players.Count)
            {
                if (Players[i].PlayersCards.Count != 0)
                {
                    nextPlIndex = i;
                    break;
                }
            }

            CurrentMovePlayer = Players[nextPlIndex];
            Players.RemoveAll(x => x.PlayersCards.Count == 0);

            NextMovePlayer = Players[(Players.IndexOf(Players.Single(x => x.Name == CurrentMovePlayer.Name)) + 1) % Players.Count];
            Position = Players.IndexOf(Players.Single(x => x.Name == CurrentMovePlayer.Name));
        }
    }
}
