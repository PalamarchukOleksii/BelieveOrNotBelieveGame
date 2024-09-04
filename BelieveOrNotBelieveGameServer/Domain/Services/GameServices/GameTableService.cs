using Domain.Abstractions.GameAbstractions;
using Domain.Common.Helpers;
using Domain.Common.Options;
using Domain.Constants;
using Domain.Dtos;
using Domain.Models.GameModels;
using Domain.Responses;

namespace Domain.Services.GameServices
{
    public class GameTableService : IGameTableService
    {
        private readonly List<GameTable> gameTables = new List<GameTable>();

        public bool CreateGame(GameTableOptions options)
        {
            GameTable? table = gameTables.Find(x => x.Options.GameName == options.GameName);

            if (table != null)
            {
                return false;
            }

            gameTables.Add(new GameTable { Options = options });
            return true;
        }

        public void EndGame(string gameName)
        {
            GameTable? table = gameTables.Find(x => x.Options.GameName == gameName);

            if (table == null)
            {
                throw new Exception("Table with that name does not exist");
            }

            table.EndGame();
        }

        public GetInfoAboutOpponentsResponse GetInfoAboutOpponents(string gameName)
        {
            GameTable? table = gameTables.Find(x => x.Options.GameName == gameName);

            if (table == null)
            {
                throw new Exception("Table with that name does not exist");
            }

            return new GetInfoAboutOpponentsResponse
            {
                Success = true,
                OpponentInfo = table.Players
                .Select(p => new OpponentInfoDto
                {
                    PlayerConnectionId = p.PlayerConnectionId,
                    Name = p.Name,
                    CardCount = p.PlayersCards.Count
                }).ToList()
            };
        }

        public GetPlayersCardsResponse GetPlayersCards(string gameName)
        {
            GameTable? table = gameTables.Find(x => x.Options.GameName == gameName);

            if (table == null)
            {
                throw new Exception("Table with that name does not exist");
            }

            return new GetPlayersCardsResponse
            {
                Success = true,
                PlayersCards = table.Players
                .Select(p => new PlayerCardsDto
                {
                    PlayerConnectionId = p.PlayerConnectionId,
                    Cards = p.PlayersCards
                }).ToList()
            };
        }

        public List<string> GetPlayersConnectionIds(string gameName)
        {
            GameTable? table = gameTables.Find(x => x.Options.GameName == gameName);

            if (table == null)
            {
                throw new Exception("Table with that name does not exist");
            }

            return new List<string>(table.Players.Select(P => P.PlayerConnectionId));
        }

        public List<string> GetPlayersWhoWinConnectionIds(string gameName)
        {
            GameTable? table = gameTables.Find(x => x.Options.GameName == gameName);

            if (table == null)
            {
                throw new Exception("Table with that name does not exist");
            }

            return new List<string>(table.PlayersWhoWin.Select(P => P.PlayerConnectionId));
        }

        public Player? GetPlayerWithConnectionId(string callerConnectionId, string gameName)
        {
            return gameTables.Single(x => x.Options.GameName == gameName).Players.Find(x => x.PlayerConnectionId == callerConnectionId);
        }

        public string JoinGame(string gameName, string username, string connectionId)
        {
            GameTable? table = gameTables.Find(x => x.Options.GameName == gameName);

            if (table == null)
            {
                throw new Exception("Table with that name does not exist");
            }

            bool result = table.JoinGameTable(username, connectionId);

            if (result)
            {
                return "Player join game";
            }

            return "Player with that name already exist";
        }

        public void MakeAssume(string gameName, bool iBelieve)
        {
            GameTable? table = gameTables.Find(x => x.Options.GameName == gameName);

            if (table == null)
            {
                throw new Exception("Table with that name does not exist");
            }

            table.MakeAssume(iBelieve);
        }

        public MakeMoveResponse MakeMove(string gameName, Player player, string cardsValue, string cardsId)
        {
            GameTable? table = gameTables.Find(x => x.Options.GameName == gameName);

            if (table == null)
            {
                throw new Exception("Table with that name does not exist");
            }

            if (player.PlayersCards.Count > 0 && player.Name == table.CurrentMovePlayer.Name && PlayerHelper.CheckIfPlayerHaveSomeCards(player, cardsId))
            {
                string playerName = player.Name;
                Move playerMove = new Move(playerName, cardsValue, cardsId);

                table.MakeMove(playerMove);

                Player nextPl = table.Players.Single(x => x.Name == table.CurrentMovePlayer.Name);

                return new MakeMoveResponse
                {
                    Success = true,
                    Message = $"{playerName} throw {playerMove.CardsId.Count} {playerMove.CardValue}",
                    CurrentMovePlayerName = nextPl.Name,
                    CurrentMovePlayerConnectionId = nextPl.PlayerConnectionId,
                    CurrentPlayerCanMakeMove = nextPl.PlayersCards.Count != 0,
                    CardsOnTableCount = table.CardsOnTable.Count
                };
            }
            else if (player.PlayersCards.Count == 0)
            {
                return new MakeMoveResponse
                {
                    Success = false,
                    Message = "You can only make an assume"
                };
            }
            else if (player.Name != table.CurrentMovePlayer.Name)
            {
                return new MakeMoveResponse
                {
                    Success = false,
                    Message = "It is not your turn"
                };
            }

            return new MakeMoveResponse
            {
                Success = false,
                Message = "You dont have this cards"
            };
        }

        public StartGameResponse StartGame(string gameName, Player player)
        {
            GameTable? table = gameTables.Find(x => x.Options.GameName == gameName);

            if (table == null)
            {
                return new StartGameResponse
                {
                    Result = "GameNotStarted",
                    Message = "Game with that name does not exist"
                };
            }

            if (table.Players.Count == 1)
            {
                return new StartGameResponse
                {
                    Result = "GameNotStarted",
                    Message = "Two players required to start the game"
                };
            }

            player.StartGame = true;

            if (table.Players.Count(x => x.StartGame) == table.Players.Count)
            {
                table.StartGame();

                string[] values = table.Options.NumOfCards switch
                {
                    24 => CardConstants.Values24,
                    36 => CardConstants.Values36,
                    _ => CardConstants.Values52,
                };

                return new StartGameResponse
                {
                    Result = "GameStarted",
                    Message = "Game started",
                    CurrentMovePlayerConnectionId = table.CurrentMovePlayer.PlayerConnectionId,
                    CurrentMovePlayerName = table.CurrentMovePlayer.Name,
                    MakeMoveValue = values.Where(x => x != "Ace").ToArray()
                };
            }

            return new StartGameResponse
            {
                Result = "WaitingForOtherPlayers",
                Message = "Waiting for other players to start the game"
            };
        }
    }
}
