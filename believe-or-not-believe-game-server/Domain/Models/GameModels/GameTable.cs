using Domain.Common.Options;
using Domain.Dtos;

namespace Domain.Models.GameModels
{
    public class GameTable
    {
        public GameTableOptions Options { get; private set; } = new GameTableOptions();
        public List<Player> Players { get; private set; } = new List<Player>();
        public List<PlayingCard> CardsOnTable { get; private set; } = new List<PlayingCard>();
        public List<PlayingCard> CardsForDiscard { get; private set; } = new List<PlayingCard>();
        public Move? Move { get; private set; }
        public CardsDeck CardsDeck { get; private set; } = new CardsDeck(36);
        public Player CurrentMovePlayer { get; private set; } = new Player();
        public Player PreviousMovePlayer { get; private set; } = new Player();
        public Player NextMovePlayer { get; private set; } = new Player();
        private int Position = 0;
        public bool GameStarted { get; private set; } = false;
        public int CountCardsForDiscard { get; private set; } = 0;
        public List<Player> PlayersWhoWin { get; private set; } = new List<Player>();

        public GameTable() { }

        public GameTable(GameTableOptions options)
        {
            Options = options;
        }

        public string CheckIfPlayerCanMakeMove(Player player, List<int> cardsId)
        {
            if (player.PlayersCards.Count == 0)
            {
                return "Zero cards";
            }

            if (player.Name != CurrentMovePlayer.Name)
            {
                return "Not this player's turn";
            }

            if (!player.CheckIfPlayerHaveSomeCards(cardsId))
            {
                return "Do not have these cards";
            }

            return "Can make move";
        }

        public bool CheckIfPlayerCanMakeAssume(Player player)
        {
            return player.Name == CurrentMovePlayer.Name;
        }

        public Player? GetPlayerByName(string playerName)
        {
            Player? player = Players.Find(x => x.Name == playerName);

            if (player is not null)
            {
                return player;
            }

            return null;
        }

        public Player? GetPlayerByConnectionId(string connectionId)
        {
            Player? player = Players.Find(x => x.PlayerConnectionId == connectionId);

            if (player is not null)
            {
                return player;
            }

            return null;
        }

        public List<ShortOpponentInfoDto> GetShortInfoAboutPlayers()
        {
            return Players.Select(x => new ShortOpponentInfoDto
            {
                PlayerConnectionId = x.PlayerConnectionId,
                Name = x.Name,
                CardCount = x.PlayersCards.Count
            }).ToList();
        }

        public (List<string> playersConnectionIds, List<string> playersWhoWinConnectionIds) GetAllConnectionId()
        {
            List<string> plConnIds = Players.Select(x => x.PlayerConnectionId).ToList();
            List<string> plWhoWinConnIds = PlayersWhoWin.Select(x => x.PlayerConnectionId).ToList();

            return (plConnIds, plWhoWinConnIds);
        }

        public bool JoinGameTable(string username, string connectionId)
        {
            if (!Players.Exists(x => x.Name == username))
            {
                Players.Add(new Player(connectionId, username));
                return true;
            }

            return false;
        }

        public List<PlayerCardsDto> GetPlayerCards()
        {
            return Players.Select(x => new PlayerCardsDto
            {
                PlayerConnectionId = x.PlayerConnectionId,
                Cards = x.PlayersCards,
            }).ToList();
        }

        public GameStateDto GetGameState()
        {
            return new GameStateDto
            {
                CurrentMovePlayerName = CurrentMovePlayer.Name,
                CurrentMovePlayerConnectionId = CurrentMovePlayer.PlayerConnectionId,
                CurrentPlayerCanMakeMove = CurrentMovePlayer.PlayersCards.Count > 0,
                CurrentPlayerCanMakeAssume = CardsOnTable.Count > 0,
                MakeMoveValue = CardsDeck.Values.ToList(),
                CardsOnTableCount = CardsOnTable.Count,
            };
        }

        public bool StartGameTable(Player player)
        {
            player.SetStartGame(true);

            if (Players.Count(x => x.StartGame) == Players.Count)
            {
                GameStarted = true;

                InitPlayersCards(Options.NumOfCards);
                InitlCalcPlayer();

                return true;
            }

            return false;
        }

        public void MakeMoveOnGameTable(Move move)
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

        public (bool EndGame, string Message) MakeAssumeOnGameTable(bool iBelieve)
        {
            bool allCardsIsCorrect = true;
            (bool EndGame, string Message) result;

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
                    result = (true, $"Game over, {CurrentMovePlayer} lose, player {PreviousMovePlayer} do not lie");
                }
                else if (Players.Count(x => x.PlayersCards.Count > 0) == 1)
                {
                    result = (true, $"Game over, {Players.Single(x => x.PlayersCards.Count > 0).Name} lose, player {PreviousMovePlayer} do not lie");
                }
                else if (Players.Single(x => x.Name == CurrentMovePlayer.Name).PlayersCards.Count > 0)
                {
                    List<Player> plWithNoCards = Players.Where(x => x.PlayersCards.Count == 0).ToList();
                    PlayersWhoWin.AddRange(plWithNoCards);

                    Players.RemoveAll(x => x.PlayersCards.Count == 0);
                    NextMovePlayer = Players[(Players.IndexOf(Players.Single(x => x.Name == CurrentMovePlayer.Name)) + 1) % Players.Count];

                    result = (false, $"{CurrentMovePlayer} player make next move, player {PreviousMovePlayer} do not lie");
                }
                else
                {
                    CalcPlayerWhenDelete();
                    result = (false, $"{CurrentMovePlayer} player make next move, player {PreviousMovePlayer} do not lie");
                }
            }
            else if (allCardsIsCorrect && !iBelieve)
            {
                Players.Single(x => x.Name == CurrentMovePlayer.Name).PlayersCards.AddRange(CardsOnTable);

                if (Players.Count(x => x.PlayersCards.Count > 0) == 1)
                {
                    result = (true, $"Game over, {Players.Single(x => x.PlayersCards.Count > 0).Name} lose, player {PreviousMovePlayer} do not lie");
                }
                else if (Players.Single(x => x.Name == NextMovePlayer.Name).PlayersCards.Count > 0)
                {
                    List<Player> plWithNoCards = Players.Where(x => x.PlayersCards.Count == 0).ToList();
                    PlayersWhoWin.AddRange(plWithNoCards);
                    Players.RemoveAll(x => x.PlayersCards.Count == 0);

                    result = (false, $"{NextMovePlayer} player make next move, player {PreviousMovePlayer} do not lie");
                    CalcPlayer();
                }
                else
                {
                    CalcPlayerWhenDelete();
                    result = (false, $"{CurrentMovePlayer} player make next move, player {PreviousMovePlayer} do not lie");
                }
            }
            else if (!allCardsIsCorrect && iBelieve)
            {
                Players.Single(x => x.Name == CurrentMovePlayer.Name).PlayersCards.AddRange(CardsOnTable);

                if (Players.Count(x => x.PlayersCards.Count > 0) == 1)
                {
                    result = new(true, $"Game over, {Players.Single(x => x.PlayersCards.Count > 0).Name} lose, player {PreviousMovePlayer} do lie");
                }
                else if (Players.Single(x => x.Name == NextMovePlayer.Name).PlayersCards.Count > 0)
                {
                    List<Player> plWithNoCards = Players.Where(x => x.PlayersCards.Count == 0).ToList();
                    PlayersWhoWin.AddRange(plWithNoCards);
                    Players.RemoveAll(x => x.PlayersCards.Count == 0);

                    result = new(false, $"{NextMovePlayer} player make next move, player {PreviousMovePlayer} dot lie");
                    CalcPlayer();
                }
                else
                {
                    CalcPlayerWhenDelete();
                    result = new(false, $"{CurrentMovePlayer} player make next move, player {PreviousMovePlayer} do lie");
                }
            }
            else
            {
                Players.Single(x => x.Name == PreviousMovePlayer.Name).PlayersCards.AddRange(CardsOnTable);

                if (Players.Count(x => x.PlayersCards.Count > 0) == 1)
                {
                    result = (true, $"Game over, {Players.Single(x => x.PlayersCards.Count > 0).Name} lose, player {PreviousMovePlayer} do lie");
                }
                else if (Players.Single(x => x.Name == CurrentMovePlayer.Name).PlayersCards.Count > 0)
                {
                    List<Player> plWithNoCards = Players.Where(x => x.PlayersCards.Count == 0).ToList();
                    PlayersWhoWin.AddRange(plWithNoCards);

                    Players.RemoveAll(x => x.PlayersCards.Count == 0);
                    NextMovePlayer = Players[(Players.IndexOf(Players.Single(x => x.Name == CurrentMovePlayer.Name)) + 1) % Players.Count];

                    result = (false, $"{CurrentMovePlayer} player make next move, player {PreviousMovePlayer} do lie");
                }
                else
                {
                    CalcPlayerWhenDelete();
                    result = (false, $"{CurrentMovePlayer} player make next move, player {PreviousMovePlayer} do lie");
                }
            }

            CardsOnTable.Clear();
            return result;
        }

        public void EndGameTable()
        {
            GameStarted = false;
            Position = CountCardsForDiscard = 0;
            Move = null;
            CardsOnTable = new List<PlayingCard>();

            Players.AddRange(PlayersWhoWin);

            PlayersWhoWin = new List<Player>();

            foreach (var player in Players)
            {
                player.OnGameEnd();
            }
        }

        private void InitPlayersCards(int numOfCards)
        {
            CardsDeck = new CardsDeck(numOfCards);
            CardsDeck.GenerateCardsDeck();
            CardsDeck.ShuffleCardsDeck();
            CardsDeck.GiveCardsToPlayers(Players);
        }

        private void InitlCalcPlayer()
        {
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
