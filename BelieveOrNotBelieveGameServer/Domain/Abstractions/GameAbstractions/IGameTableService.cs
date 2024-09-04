using Domain.Common.Options;
using Domain.Models.GameModels;
using Domain.Responses;

namespace Domain.Abstractions.GameAbstractions
{
    public interface IGameTableService
    {
        public bool CreateGame(GameTableOptions options);
        public string JoinGame(string gameName, string username, string connectionId);
        public StartGameResponse StartGame(string gameName, Player player);
        public MakeMoveResponse MakeMove(string gameName, Player player, string cardsValue, string cardsId);
        public void MakeAssume(string gameName, bool iBelieve);
        public void EndGame(string gameName);
        Player? GetPlayerWithConnectionId(string callerConnectionId, string gameName);
        GetInfoAboutOpponentsResponse GetInfoAboutOpponents(string gameName);
        GetPlayersCardsResponse GetPlayersCards(string gameName);
        List<string> GetPlayersConnectionIds(string gameName);
        List<string> GetPlayersWhoWinConnectionIds(string gameName);
    }
}
