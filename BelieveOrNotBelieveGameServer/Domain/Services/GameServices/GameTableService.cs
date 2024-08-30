using Domain.Abstractions.GameAbstractions;
using Domain.Common.Options;
using Domain.Constants;
using Domain.Dtos.Responses;
using Domain.Models.GameModels;

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

        public Player? GetPlayerWithConnectionId(string callerConnectionId, string gameName)
        {
            return gameTables.Single(x => x.Options.GameName == gameName).Players.Single(x => x.PlayerConnectionId == callerConnectionId);
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

        public void MakeMove(string gameName, Move move)
        {
            GameTable? table = gameTables.Find(x => x.Options.GameName == gameName);

            if (table == null)
            {
                throw new Exception("Table with that name does not exist");
            }

            table.MakeMove(move);
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
                    MakeMoveValue = values
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
