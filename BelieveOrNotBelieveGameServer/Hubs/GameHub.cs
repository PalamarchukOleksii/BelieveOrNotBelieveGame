using Microsoft.AspNetCore.SignalR;
using BelieveOrNotBelieveGameServer.Models;
using BelieveOrNotBelieveGameServer.Dtos;

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
                await Clients.All.SendAsync("ReceiveJoin", $"Player {username} joined game");

                await SendInfoAboutOpponents();
            }
            else
            {
                await Clients.Caller.SendAsync("ReceiveNotJoin", $"Player with username {username} is alreade exist, change username");
            }
        }

        public async Task StartGame(string numOfCard)
        {
            GameTable.Players.Single(x => x.PlayerConnectionId == Context.ConnectionId).StartGame = true;
            if(GameTable.Players.Count == 1)
            {
                await Clients.All.SendAsync("ReceiveNotStart", "At least two players are required");
                GameTable.Players.Single(x => x.PlayerConnectionId == Context.ConnectionId).StartGame = false;
            }
            else if (GameTable.Players.Exists(x => x.PlayerConnectionId == Context.ConnectionId) && GameTable.Players.Count(x => x.StartGame) == GameTable.Players.Count)
            {
                GameTable.StartGame(Convert.ToInt32(numOfCard));

                await SendPlayersCards();
                await SendInfoAboutOpponents();

                await Clients.Client(GameTable.Players.Single(x => x.Name == GameTable.CurrentMovePlayerName).PlayerConnectionId).SendAsync("ReceiveStartMove", "You start the game");

                string[] values;

                if (Convert.ToInt32(numOfCard) == 24)
                {
                    values = ["9", "10", "Jack", "Queen", "King", "Ace"];
                }
                else if (Convert.ToInt32(numOfCard) == 36)
                {
                    values = ["6", "7", "8", "9", "10", "Jack", "Queen", "King", "Ace"];
                }
                else
                {
                    values = ["2", "3", "4", "5", "6", "7", "8", "9", "10", "Jack", "Queen", "King", "Ace"];
                }

                await Clients.All.SendAsync("ReceiveMakeMoveValues", values);
            }
        }

        public async Task MakeMove(string cardsValue, string cardsId)
        {
            Player pl = GameTable.Players.Single(x => x.PlayerConnectionId == Context.ConnectionId);
            if (pl.PlayersCards.Count > 0 && pl.Name == GameTable.CurrentMovePlayerName && pl.CheckIfPlayerHaveSomeCards(cardsId))
            {
                string playerName = pl.Name;
                Move playerMove = new Move(playerName, cardsValue, cardsId);

                GameTable.MakeMove(playerMove);

                await Clients.All.SendAsync("RecieveMove", $"{playerName} throw {playerMove.CardsId.Count} {playerMove.CardValue}");

                Player nextPl = GameTable.Players.Single(x => x.Name == GameTable.CurrentMovePlayerName);

                if (nextPl.PlayersCards.Count == 0)
                {
                    await Clients.Client(nextPl.PlayerConnectionId).SendAsync("ReceiveNextAssume", "You make assume");
                }
                else
                {
                    await Clients.Client(nextPl.PlayerConnectionId).SendAsync("ReceiveNextMoveAssume", "You make assume or move");
                }

                await Clients.All.SendAsync("ReceiveCardOnTableCount", GameTable.CardsOnTable.Count);

                await SendPlayersCards();
                await SendInfoAboutOpponents();
            }
            else if(pl.PlayersCards.Count == 0)
            {
                await Clients.Caller.SendAsync("RecieveNotMove", "You can only make an assume");
            }
        }

        public async Task MakeAssume(bool iBelieve)
        {
            if (GameTable.Players.Single(x => x.PlayerConnectionId == Context.ConnectionId).Name == GameTable.CurrentMovePlayerName)
            {
                Tuple<int, string> result = GameTable.MakeAssume(iBelieve);

                if (result.Item1 == 5)
                {
                    GameTable.GameStarted = false;
                    await Clients.All.SendAsync("ReceiveGameOver", result.Item2);
                    GameTable.EndGame();
                }
                else
                {
                    await Clients.All.SendAsync("ReceiveAssume", result.Item2);
                    await Clients.Client(GameTable.Players.Single(x => x.Name == GameTable.CurrentMovePlayerName).PlayerConnectionId).SendAsync("ReceiveFirstMove", "You make move");
                }

                await SendPlayersCards();
                await SendInfoAboutOpponents();

                await Clients.All.SendAsync("ReceiveCardOnTableCount", GameTable.CardsOnTable.Count);
                await Clients.All.SendAsync("ReceiveDiscardCardsCount", GameTable.CountCardsForDiscard);
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
            List<OpponentInfoDto> opponentsInfo = GameTable.Players
             .Select(p => new OpponentInfoDto
             {
                 Name = p.Name,
                 CardCount = p.PlayersCards.Count
             })
             .ToList();

            for (int i = 0; i < GameTable.Players.Count; i++)
            {
                await Clients.Client(GameTable.Players[i].PlayerConnectionId).SendAsync("ReceiveOpponentsInfo", opponentsInfo.Where(x => x.Name != GameTable.Players[i].Name));
            }

            for (int i = 0; i < GameTable.PlayersWhoWin.Count; i++)
            {
                await Clients.Client(GameTable.PlayersWhoWin[i].PlayerConnectionId).SendAsync("ReceiveOpponentsInfo", opponentsInfo.Where(x => x.Name != GameTable.PlayersWhoWin[i].Name));
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (GameTable.Players.Exists(x => x.PlayerConnectionId == Context.ConnectionId))
            {
                if (GameTable.GameStarted)
                {
                    GameTable.GameStarted = false;
                    await Clients.All.SendAsync("ReceiveGameOver", $"Game over, {GameTable.Players.Single(x => x.PlayerConnectionId == Context.ConnectionId).Name} lose");
                }

                GameTable.Players.Remove(GameTable.Players.Single(x => x.PlayerConnectionId == Context.ConnectionId));
                GameTable.EndGame();

                await SendInfoAboutOpponents();
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
}