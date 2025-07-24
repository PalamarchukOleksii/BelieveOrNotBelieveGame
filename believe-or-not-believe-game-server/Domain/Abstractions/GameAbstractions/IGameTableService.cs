using Domain.Models.GameModels;

namespace Domain.Abstractions.GameAbstractions
{
    public interface IGameTableService
    {
        public GameTable? CreateGameTable(string gameName, int numOfCards, int maxNumOfPlayers, bool addBot);
        public GameTable? GetGameTableByName(string gameName);
        public bool DeleteGameTableByName(string gameName);
        public GameTable? GetGameTableByConnectionId(string connectionId);
    }
}
