namespace BelieveOrNotBelieveGameServer.Models
{
    public class GameTable
    {
        public List<Player> Players { get; set; } = new List<Player>();
        public List<PlayingCard> PlayedCards { get; set; } = new List<PlayingCard>();
        public List<PlayingCard> CardsOnTable { get; set; } = new List<PlayingCard>();
        public Move? Move { get; set; }
        public string CurrentMovePlayerName { get; set; } = string.Empty;
        public string PreviousMovePlayerName { get; set; } = string.Empty;
        public string NextMovePlayerName { get; set; } = string.Empty;
        private int Position { get; set; }
        public bool GameStarted { get; set; } = false;

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

            for(int i = CardsOnTable.Count - 1; i >= CardsOnTable.Count - 1 - Move?.CardsId.Count; i--)
            {
                if (CardsOnTable[i].CardValue != Move?.CardValue)
                {
                    allCardsIsCorrect = false;
                    break;
                }
            }

            if (allCardsIsCorrect && iBelieve)
            {
                PlayedCards = CardsOnTable;
                result = new Tuple<int, string>(1, $"{CurrentMovePlayerName} player make next move, player {PreviousMovePlayerName} do not lie;");
            }
            else if (allCardsIsCorrect && !iBelieve)
            {
                Players.Single(x => x.Name == CurrentMovePlayerName).PlayersCards.AddRange(CardsOnTable);
                result = new Tuple<int, string>(2, $"{NextMovePlayerName} player make next move, player {PreviousMovePlayerName} do not lie;");

                CalcPlayer();
            }
            else if (!allCardsIsCorrect && iBelieve)
            {
                Players.Single(x => x.Name == CurrentMovePlayerName).PlayersCards.AddRange(CardsOnTable);
                result = new Tuple<int, string>(3, $"{NextMovePlayerName} player make next move, player {PreviousMovePlayerName} do lie;");

                CalcPlayer();
            }
            else
            {
                Players.Single(x => x.Name == PreviousMovePlayerName).PlayersCards.AddRange(CardsOnTable);
                result = new Tuple<int, string>(4, $"{CurrentMovePlayerName} player make next move, player {PreviousMovePlayerName} do lie;");
            }

            CardsOnTable.Clear();
            return result;
        }

        private void CalcPlayer()
        {
            PreviousMovePlayerName = CurrentMovePlayerName;
            CurrentMovePlayerName = NextMovePlayerName;
            Position++;
            if (Position == Players.Count - 1)
            {
                NextMovePlayerName = Players[0].Name;
            }
            else
            {
                NextMovePlayerName = Players[Position + 1].Name;
            }
        }
    }
}
