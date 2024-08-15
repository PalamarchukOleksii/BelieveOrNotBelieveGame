using Microsoft.AspNetCore.SignalR;
using BelieveOrNotBelieveGameServer.Models;

namespace BelieveOrNotBelieveGameServer.Hubs
{
    public class GameHub : Hub
    {
        static GameTable GameTable { get; set; } = new GameTable();

        public async Task JoinGameTable(string username)
        {
            if (!GameTable.Players.Exists(x => x.Name == username))
            {
                GameTable.Players.Add(new Player { PlayerConnectionId = Context.ConnectionId, Name = username });
                await Clients.All.SendAsync("ReceiveJoin", $"Player {username} joined game;");
            }
            else
            {
                await Clients.Caller.SendAsync("ReceiveNotJoin", $"Player with username {username} is alreade exist, change username;");
            }
        }

        public async Task StartGame(string numOfCard)
        {
            GameTable.Players.Single(x => x.PlayerConnectionId == Context.ConnectionId).StartGame = true;
            if(GameTable.Players.Count == 1)
            {
                await Clients.All.SendAsync("ReceiveNotStart", "At least two players are required;");
                GameTable.Players.Single(x => x.PlayerConnectionId == Context.ConnectionId).StartGame = false;
            }
            else if (GameTable.Players.Exists(x => x.PlayerConnectionId == Context.ConnectionId) && GameTable.Players.Count(x => x.StartGame) == GameTable.Players.Count)
            {
                GameTable.StartGame(Convert.ToInt32(numOfCard));

                await SendPlayersCards();
                await SendInfoAboutOpponents();

                await Clients.Client(GameTable.Players.Single(x => x.Name == GameTable.CurrentMovePlayerName).PlayerConnectionId).SendAsync("ReceiveStart", "You start the game;");
            }
        }

        public async Task MakeMove(string cardsValue, string cardsId)
        {
            Player pl = GameTable.Players.Single(x => x.PlayerConnectionId == Context.ConnectionId);
            if (pl.Name == GameTable.CurrentMovePlayerName || pl.CheckIfPlayerHaveSomeCards(cardsId))
            {
                string playerName = pl.Name;
                Move playerMove = new Move(playerName, cardsValue, cardsId);

                GameTable.MakeMove(playerMove);

                await Clients.All.SendAsync("RecieveMove", $"{playerName} throw {playerMove.CardsId.Count} {playerMove.CardValue}");
                await Clients.Client(GameTable.Players.Single(x => x.Name == GameTable.CurrentMovePlayerName).PlayerConnectionId).SendAsync("ReceiveNextMove", "You make assume or move;");

                await SendPlayersCards();
                await SendInfoAboutOpponents();
            }
        }

        public async Task MakeAssume(bool iBelieve)
        {
            if (GameTable.Players.Single(x => x.PlayerConnectionId == Context.ConnectionId).Name == GameTable.CurrentMovePlayerName)
            {
                Tuple<int, string> result = GameTable.MakeAssume(iBelieve);

                await SendPlayersCards();
                await SendInfoAboutOpponents();

                await Clients.All.SendAsync("ReceiveAssume", result.Item2);
                await Clients.Client(GameTable.Players.Single(x => x.Name == GameTable.CurrentMovePlayerName).PlayerConnectionId).SendAsync("ReceiveFirstMove", "You make move;");
            }
        }

        private async Task SendPlayersCards()
        {
            for (int i = 0; i < GameTable.Players.Count; i++)
            {
                await Clients.Client(GameTable.Players[i].PlayerConnectionId).SendAsync("ReceiveCard", GameTable.Players[i].PlayersCards);
            }
        }

        public async Task SendInfoAboutOpponents()
        {
            for (int i = 0; i < GameTable.Players.Count; i++)
            {
                await Clients.Client(GameTable.Players[i].PlayerConnectionId).SendAsync("ReceiveOpponentsInfo", GameTable.Players.Where(x => x.PlayerConnectionId != GameTable.Players[i].PlayerConnectionId));
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (GameTable.Players.Exists(x => x.PlayerConnectionId == Context.ConnectionId))
            {
                if (GameTable.GameStarted)
                {
                    GameTable.GameStarted = false;
                    await Clients.All.SendAsync("ReceiveGameOver", $"Game over, {GameTable.Players.Single(x => x.PlayerConnectionId == Context.ConnectionId).Name} lose;");
                }
                GameTable.Players.Remove(GameTable.Players.Single(x => x.PlayerConnectionId == Context.ConnectionId));
                await SendInfoAboutOpponents();
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
}