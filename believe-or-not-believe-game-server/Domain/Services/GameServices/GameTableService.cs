using System.Collections.Concurrent;
using Domain.Abstractions.GameAbstractions;
using Domain.Models.GameModels;

namespace Domain.Services.GameServices
{
    public class GameTableService : IGameTableService
    {
        private readonly ConcurrentDictionary<string, GameTable> _gameTables = new();
        private readonly ConcurrentDictionary<string, GameTable> _connectionIdToTable = new();

        public GameTable? CreateGameTable(string gameName, int numOfCards, int maxNumOfPlayers, bool addBot)
        {
            var newGame = new GameTable(gameName, numOfCards, maxNumOfPlayers, addBot);
            bool added = _gameTables.TryAdd(gameName, newGame);
            return added ? newGame : null;
        }
        
        public bool DeleteGameTableByName(string gameName)
        {
            return _gameTables.TryRemove(gameName, out _);
        }
        
        public GameTable? GetGameTableByName(string gameName)
        {
            _gameTables.TryGetValue(gameName, out var table);
            return table;
        }

        public GameTable? GetGameTableByConnectionId(string connectionId)
        {
            _connectionIdToTable.TryGetValue(connectionId, out var table);
            return table;
        }
        
        public bool PlayerJoinGameTable(string username, string connectionId, string gameName)
        {
            if (!_gameTables.TryGetValue(gameName, out var table))
                return false;
            
            if (table.GetPlayerByName(username) != null)
                return false;
            
            bool joinResult = table.JoinGameTable(username, connectionId);
            if (joinResult)
            {
                _connectionIdToTable.TryAdd(connectionId, table);
            }

            return joinResult;
        }
        
        public bool PlayerLeaveGameTable(string connectionId)
        {
            if (!_connectionIdToTable.TryGetValue(connectionId, out var table))
                return false;

            bool leaveResult = table.LeaveGameTable(connectionId);
            if (leaveResult)
            {
                _connectionIdToTable.TryRemove(connectionId, out _);
            }

            return leaveResult;
        }
    }
}
