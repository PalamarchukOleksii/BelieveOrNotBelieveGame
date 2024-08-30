using Domain.Common.Options;
using Domain.Dtos.Responses;
using Domain.Models.GameModels;

namespace Domain.Abstractions.GameAbstractions
{
    public interface IGameTableService
    {
        public bool CreateGame(GameTableOptions options);
        public string JoinGame(string gameName, string username, string connectionId);
        public StartGameResponse StartGame(string gameName, Player player);
        public void MakeMove(string gameName, Move move);
        public void MakeAssume(string gameName, bool iBelieve);
        public void EndGame(string gameName);
        Player? GetPlayerWithConnectionId(string callerConnectionId, string gameName);
    }
}
