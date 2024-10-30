using Domain.Common.Options;
using Domain.Models.GameModels;

namespace Domain.Abstractions.GameAbstractions
{
    public interface IGameTableService
    {
        public GameTable? CreateGameTable(GameTableOptions gameTableOptions);
        public GameTable? GetGameTableByName(string gameName);
        public bool DeleteGameTableByName(string gameName);
        public GameTable? GetGameTableByConnectionId(string connectionId);
    }
}
