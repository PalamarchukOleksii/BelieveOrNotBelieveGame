namespace BelieveOrNotBelieveGameServer.Models
{
    public class GameTable
    {
        public List<Player> Players { get; set; } = new List<Player>();
        public List<PlayingCard> CardsOnTable { get; set; } = new List<PlayingCard>();
        public Move? Move { get; set; }
        public string CurrentMovePlayerName { get; set; } = string.Empty;
        public string PreviousMovePlayerName { get; set; } = string.Empty;
        public string NextMovePlayerName { get; set; } = string.Empty;
        private int Position { get; set; } = 0;
        public bool GameStarted { get; set; } = false;
        public int CountCardsForDiscard {  get; set; } = 0;
        public List<Player> PlayersWhoWin { get; set; } = new List<Player>();

        public void StartGame(int numOfCards)
        {
            GameStarted = true;
            CardsDeck deck = new CardsDeck(numOfCards);
            deck.ShuffleCards();
            deck.GiveCardsToPlayers(Players);

            Random rnd = new Random();
            Position = rnd.Next(0, Players.Count - 1);

            CurrentMovePlayerName = Players[Position].Name;
            if (Position == Players.Count - 1)
            {
                NextMovePlayerName = Players[0].Name;
            }
            else
            {
                NextMovePlayerName = Players[Position + 1].Name;
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

        public Tuple<int, string> MakeAssume(bool iBelieve)
        {
            bool allCardsIsCorrect = true;
            Tuple<int, string> result;

            for(int i = CardsOnTable.Count - 1; i >= CardsOnTable.Count - Move?.CardsId.Count; i--)
            {
                if (CardsOnTable[i].CardValue != Move?.CardValue)
                {
                    allCardsIsCorrect = false;
                    break;
                }
            }

            if (allCardsIsCorrect && iBelieve)
            {
                CountCardsForDiscard = CardsOnTable.Count;

                if (Players.TrueForAll(x => x.PlayersCards.Count == 0))
                {
                    result = new Tuple<int, string>(5, $"Game over, {CurrentMovePlayerName} lose, player {PreviousMovePlayerName} do not lie");
                }
                else if (Players.Count(x => x.PlayersCards.Count > 0) == 1)
                {
                    result = new Tuple<int, string>(5, $"Game over, {Players.Single(x => x.PlayersCards.Count > 0).Name} lose, player {PreviousMovePlayerName} do not lie");
                }
                else if (Players.Single(x => x.Name == CurrentMovePlayerName).PlayersCards.Count > 0)
                {
                    List<Player> plWithNoCards = Players.Where(x => x.PlayersCards.Count == 0).ToList();
                    PlayersWhoWin.AddRange(plWithNoCards);

                    Players.RemoveAll(x => x.PlayersCards.Count == 0);
                    NextMovePlayerName = Players[(Players.IndexOf(Players.Single(x => x.Name == CurrentMovePlayerName)) + 1) % Players.Count].Name;

                    result = new Tuple<int, string>(1, $"{CurrentMovePlayerName} player make next move, player {PreviousMovePlayerName} do not lie");
                }
                else
                {
                    CalcPlayerWhenDelete();
                    result = new Tuple<int, string>(1, $"{CurrentMovePlayerName} player make next move, player {PreviousMovePlayerName} do not lie");
                }
            }
            else if (allCardsIsCorrect && !iBelieve)
            {
                Players.Single(x => x.Name == CurrentMovePlayerName).PlayersCards.AddRange(CardsOnTable);

                if (Players.Count(x => x.PlayersCards.Count > 0) == 1)
                {
                    result = new Tuple<int, string>(5, $"Game over, {Players.Single(x => x.PlayersCards.Count > 0).Name} lose, player {PreviousMovePlayerName} do not lie");
                }
                else if (Players.Single(x => x.Name == NextMovePlayerName).PlayersCards.Count > 0)
                {
                    List<Player> plWithNoCards = Players.Where(x => x.PlayersCards.Count == 0).ToList();
                    PlayersWhoWin.AddRange(plWithNoCards);
                    Players.RemoveAll(x => x.PlayersCards.Count == 0);

                    result = new Tuple<int, string>(2, $"{NextMovePlayerName} player make next move, player {PreviousMovePlayerName} do not lie");
                    CalcPlayer();
                }
                else
                {
                    CalcPlayerWhenDelete();
                    result = new Tuple<int, string>(2, $"{CurrentMovePlayerName} player make next move, player {PreviousMovePlayerName} do not lie");
                }
            }
            else if (!allCardsIsCorrect && iBelieve)
            {
                Players.Single(x => x.Name == CurrentMovePlayerName).PlayersCards.AddRange(CardsOnTable);

                if (Players.Count(x => x.PlayersCards.Count > 0) == 1)
                {
                    result = new Tuple<int, string>(5, $"Game over, {Players.Single(x => x.PlayersCards.Count > 0).Name} lose, player {PreviousMovePlayerName} do lie");
                }
                else if (Players.Single(x => x.Name == NextMovePlayerName).PlayersCards.Count > 0)
                {
                    List<Player> plWithNoCards = Players.Where(x => x.PlayersCards.Count == 0).ToList();
                    PlayersWhoWin.AddRange(plWithNoCards);
                    Players.RemoveAll(x => x.PlayersCards.Count == 0);

                    result = new Tuple<int, string>(3, $"{NextMovePlayerName} player make next move, player {PreviousMovePlayerName} dot lie");
                    CalcPlayer();
                }
                else
                {
                    CalcPlayerWhenDelete();
                    result = new Tuple<int, string>(3, $"{CurrentMovePlayerName} player make next move, player {PreviousMovePlayerName} do lie");
                }
            }
            else
            {
                Players.Single(x => x.Name == PreviousMovePlayerName).PlayersCards.AddRange(CardsOnTable);

                if (Players.Count(x => x.PlayersCards.Count > 0) == 1)
                {
                    result = new Tuple<int, string>(5, $"Game over, {Players.Single(x => x.PlayersCards.Count > 0).Name} lose, player {PreviousMovePlayerName} do lie");
                }
                else if (Players.Single(x => x.Name == CurrentMovePlayerName).PlayersCards.Count > 0)
                {
                    List<Player> plWithNoCards = Players.Where(x => x.PlayersCards.Count == 0).ToList();
                    PlayersWhoWin.AddRange(plWithNoCards);

                    Players.RemoveAll(x => x.PlayersCards.Count == 0);
                    NextMovePlayerName = Players[Players.IndexOf(Players.Single(x => x.Name == CurrentMovePlayerName)) + 1 % Players.Count].Name;

                    result = new Tuple<int, string>(4, $"{CurrentMovePlayerName} player make next move, player {PreviousMovePlayerName} do lie");
                }
                else
                {
                    CalcPlayerWhenDelete();
                    result = new Tuple<int, string>(4, $"{CurrentMovePlayerName} player make next move, player {PreviousMovePlayerName} do lie");
                }
            }

            CardsOnTable.Clear();
            return result;
        }

        public void EndGame()
        {
            GameStarted = false;
            Position = CountCardsForDiscard = 0;
            CurrentMovePlayerName = PreviousMovePlayerName = NextMovePlayerName = string.Empty;
            Move = null;
            CardsOnTable = new List<PlayingCard>();

            Players.AddRange(PlayersWhoWin);

            PlayersWhoWin = new List<Player>();

            foreach(var player in Players)
            {
                player.StartGame = false;
                player.PlayersCards = new List<PlayingCard>();
            }
    }

        private void CalcPlayer()
        {
            PreviousMovePlayerName = CurrentMovePlayerName;
            CurrentMovePlayerName = NextMovePlayerName;

            Position = (Position + 1) % Players.Count;

            NextMovePlayerName = Players[(Position + 1) % Players.Count].Name;
        }

        private void CalcPlayerWhenDelete()
        {
            List<Player> plWithNoCards = Players.Where(x => x.PlayersCards.Count == 0).ToList();
            PlayersWhoWin.AddRange(plWithNoCards);

            int currentPlIndex = Players.IndexOf(Players.Single(x => x.Name == CurrentMovePlayerName));
            int nextPlIndex = 0;

            for (int i = currentPlIndex + 1 % Players.Count, c = 0; c < Players.Count; c++, i = (i + 1) % Players.Count)
            {
                if (Players[i].PlayersCards.Count != 0)
                {
                    nextPlIndex = i;
                    break;
                }
            }

            CurrentMovePlayerName = Players[nextPlIndex].Name;
            Players.RemoveAll(x => x.PlayersCards.Count == 0);

            NextMovePlayerName = Players[(Players.IndexOf(Players.Single(x => x.Name == CurrentMovePlayerName)) + 1) % Players.Count].Name;
            Position = Players.IndexOf(Players.Single(x => x.Name == CurrentMovePlayerName));
        }
    }
}
