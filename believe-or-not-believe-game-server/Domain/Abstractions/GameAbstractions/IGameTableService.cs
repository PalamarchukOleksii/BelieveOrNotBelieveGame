using Domain.Models.GameModels;

namespace Domain.Abstractions.GameAbstractions
{
    public interface IGameTableService
    {
        GameTable? CreateGameTable(string gameName, int numOfCards, int maxNumOfPlayers, bool addBot);
        GameTable? GetGameTableByName(string gameName);
        bool DeleteGameTableByName(string gameName);
        GameTable? GetGameTableByConnectionId(string connectionId);
        bool PlayerJoinGameTable(string username, string connectionId, string gameName);
        bool PlayerLeaveGameTable(string connectionId);
    }
}
