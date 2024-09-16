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

        //public void EndGame(string gameName)
        //{
        //    GameTable? table = gameTables.Find(x => x.Options.GameName == gameName);

        //    if (table == null)
        //    {
        //        throw new Exception("Table with that name does not exist");
        //    }

        //    table.EndGame();
        //}

        //public GetInfoAboutOpponentsResponse GetInfoAboutOpponents(string gameName)
        //{
        //    GameTable? table = gameTables.Find(x => x.Options.GameName == gameName);

        //    if (table == null)
        //    {
        //        throw new Exception("Table with that name does not exist");
        //    }

        //    return new GetInfoAboutOpponentsResponse
        //    {
        //        Success = true,
        //        OpponentInfo = table.Players
        //        .Select(p => new OpponentInfoDto
        //        {
        //            PlayerConnectionId = p.PlayerConnectionId,
        //            Name = p.Name,
        //            CardCount = p.PlayersCards.Count
        //        }).ToList()
        //    };
        //}

        //public GetPlayersCardsResponse GetPlayersCards(string gameName)
        //{
        //    GameTable? table = gameTables.Find(x => x.Options.GameName == gameName);

        //    if (table == null)
        //    {
        //        throw new Exception("Table with that name does not exist");
        //    }

        //    return new GetPlayersCardsResponse
        //    {
        //        Success = true,
        //        PlayersCards = table.Players
        //        .Select(p => new PlayerCardsDto
        //        {
        //            PlayerConnectionId = p.PlayerConnectionId,
        //            Cards = p.PlayersCards
        //        }).ToList()
        //    };
        //}

        //public List<string> GetPlayersConnectionIds(string gameName)
        //{
        //    GameTable? table = gameTables.Find(x => x.Options.GameName == gameName);

        //    if (table == null)
        //    {
        //        throw new Exception("Table with that name does not exist");
        //    }

        //    return new List<string>(table.Players.Select(P => P.PlayerConnectionId));
        //}

        //public List<string> GetPlayersWhoWinConnectionIds(string gameName)
        //{
        //    GameTable? table = gameTables.Find(x => x.Options.GameName == gameName);

        //    if (table == null)
        //    {
        //        throw new Exception("Table with that name does not exist");
        //    }

        //    return new List<string>(table.PlayersWhoWin.Select(P => P.PlayerConnectionId));
        //}

        //public Player? GetPlayerWithConnectionId(string callerConnectionId, string gameName)
        //{
        //    return gameTables.Single(x => x.Options.GameName == gameName).Players.Find(x => x.PlayerConnectionId == callerConnectionId);
        //}

        //public void MakeAssume(string gameName, bool iBelieve)
        //{
        //    GameTable? table = gameTables.Find(x => x.Options.GameName == gameName);

        //    if (table == null)
        //    {
        //        throw new Exception("Table with that name does not exist");
        //    }

        //    table.MakeAssume(iBelieve);
        //}
    }
}
