using Domain.Abstractions.GameAbstractions;
using Domain.Common.Options;
using Domain.Models.GameModels;

namespace Domain.Services.GameServices
{
    public class GameTableService : IGameTableService
    {
        private readonly List<GameTable> gameTables = new List<GameTable>();

        public GameTable? CreateGameTable(GameTableOptions gameTableOptions)
        {
            GameTable? table = gameTables.Find(x => x.Options.GameName == gameTableOptions.GameName);

            if (table is not null)
            {
                return null;
            }

            GameTable newGame = new GameTable(gameTableOptions);
            gameTables.Add(newGame);
            return newGame;
        }

        public bool DeleteGameTableByName(string gameName)
        {
            GameTable? table = gameTables.Find(x => x.Options.GameName == gameName);

            if (table is null)
            {
                return false;
            }

            return gameTables.Remove(table);
        }

        public GameTable? GetGameTableByName(string gameName)
        {
            GameTable? table = gameTables.Find(x => x.Options.GameName == gameName);

            if (table is null)
            {
                return default;
            }

            return table;
        }

        public GameTable? GetGameTableByConnectionId(string connectionId)
        {
            GameTable? table = gameTables.Find(x => x.Players.Exists(y => y.PlayerConnectionId == connectionId));

            if (table is null)
            {
                return default;
            }

            return table;
        }
    }
}
