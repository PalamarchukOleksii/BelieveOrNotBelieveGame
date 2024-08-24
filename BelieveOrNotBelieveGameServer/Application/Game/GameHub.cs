using Microsoft.AspNetCore.SignalR;
using BelieveOrNotBelieveGameServer.Models;
using BelieveOrNotBelieveGameServer.Application.Game.Helpers;
using BelieveOrNotBelieveGameServer.Common.Constants;
using BelieveOrNotBelieveGameServer.Common.Helpers;
using BelieveOrNotBelieveGameServer.Models.BotModels;

namespace BelieveOrNotBelieveGameServer.Application.Game
{
    public class GameHub : Hub
    {
        static GameTable GameTable { get; set; } = new GameTable();

        private readonly string ReceiveAssume = "ReceiveAssume";

        public async Task JoinGameTable(string username)
        {
            if (!GameTable.Players.Exists(x => x.Name == username))
            {
                GameTable.Players.Add(new Player { PlayerConnectionId = Context.ConnectionId, Name = username });
                await Clients.All.SendAsync("ReceiveJoin", $"Player {username} joined game");

                await SendInfoAboutOpponentsAsync();
            }
            else
            {
                await Clients.Caller.SendAsync("ReceiveNotJoin", $"Player with username {username} is alreade exist, change username");
            }
        }

        public async Task StartGame(string numOfCard, Bot bot)//Add bot settings
        {
            GameTable.Players.Single(x => x.PlayerConnectionId == Context.ConnectionId).StartGame = true;
            if (GameTable.Players.Count == 1)
            {
                await Clients.All.SendAsync("ReceiveNotStart", "At least two players are required");
                GameTable.Players.Single(x => x.PlayerConnectionId == Context.ConnectionId).StartGame = false;
            }
            else if (GameTable.Players.Exists(x => x.PlayerConnectionId == Context.ConnectionId) && GameTable.Players.Count(x => x.StartGame) == GameTable.Players.Count)
            {
                GameTable.StartGame(Convert.ToInt32(numOfCard));

                await GameInfoHelper.SendPlayersCards(GameTable, Clients);
                await SendInfoAboutOpponentsAsync();

                await Clients.Client(GameTable.Players.Single(x => x.Name == GameTable.CurrentMovePlayerName).PlayerConnectionId).SendAsync("ReceiveStartMove", "You start the game");
                await Clients.AllExcept(GameTable.Players.Single(x => x.Name == GameTable.CurrentMovePlayerName).PlayerConnectionId).SendAsync("ReceiveCurrentMovePlayer", $"{GameTable.Players.Single(x => x.Name == GameTable.CurrentMovePlayerName).Name} make move");

                if (Convert.ToInt32(numOfCard) == 24)
                await Clients.Client(GameTable.Players.Single(x => x.Name == GameTable.CurrentMovePlayer.Name).PlayerConnectionId).SendAsync("ReceiveStartMove", "You start the game");
                string[] values = Convert.ToInt32(numOfCard) switch
                {
                    24 => CardConstants.Values24,
                    36 => CardConstants.Values36,
                    _ => CardConstants.Values52,
                };
                await Clients.All.SendAsync("ReceiveMakeMoveValues", values);
            }
        }

        public async Task MakeMove(string cardsValue, string cardsId)
        {
            Player pl = GameTable.Players.Single(x => x.PlayerConnectionId == Context.ConnectionId);
            if (pl.PlayersCards.Count > 0 && pl.Name == GameTable.CurrentMovePlayer.Name && PlayerHelper.CheckIfPlayerHaveSomeCards(pl, cardsId))
            {
                string playerName = pl.Name;
                Move playerMove = new Move(playerName, cardsValue, cardsId);

                GameTable.MakeMove(playerMove);

                await Clients.All.SendAsync("RecieveMove", $"{playerName} throw {playerMove.CardsId.Count} {playerMove.CardValue}");

                Player nextPl = GameTable.Players.Single(x => x.Name == GameTable.CurrentMovePlayer.Name);

                if (nextPl.PlayersCards.Count == 0)
                {
                    await Clients.Client(nextPl.PlayerConnectionId).SendAsync("ReceiveNextAssume", "You make assume");
                    await Clients.AllExcept(GameTable.Players.Single(x => x.Name == GameTable.CurrentMovePlayerName).PlayerConnectionId).SendAsync("ReceiveCurrentMovePlayer", $"{GameTable.Players.Single(x => x.Name == GameTable.CurrentMovePlayerName).Name} make move");
                }
                else
                {
                    await Clients.Client(nextPl.PlayerConnectionId).SendAsync("ReceiveNextMoveAssume", "You make assume or move");
                    await Clients.AllExcept(GameTable.Players.Single(x => x.Name == GameTable.CurrentMovePlayerName).PlayerConnectionId).SendAsync("ReceiveCurrentMovePlayer", $"{GameTable.Players.Single(x => x.Name == GameTable.CurrentMovePlayerName).Name} make assume or move");
                }

                await Clients.All.SendAsync("ReceiveCardOnTableCount", GameTable.CardsOnTable.Count);

                await GameInfoHelper.SendPlayersCards(GameTable, Clients);
                await SendInfoAboutOpponentsAsync();
            }
            else if (pl.PlayersCards.Count == 0)
            {
                await Clients.Caller.SendAsync("RecieveNotMove", "You can only make an assume");
            }
        }

        public async Task MakeAssume(bool iBelieve)
        {
            if (GameTable.Players.Single(x => x.PlayerConnectionId == Context.ConnectionId).Name == GameTable.CurrentMovePlayer.Name)
            {
                (int ResultId, string ClientMessage) = GameTable.MakeAssume(iBelieve);

                if (ResultId == 5)
                {
                    GameTable.GameStarted = false;
                    await Clients.All.SendAsync("ReceiveGameOver", ClientMessage);
                    GameTable.EndGame();
                }
                else
                {
                    await Clients.All.SendAsync("ReceiveAssume", ClientMessage);
                    await Clients.Client(GameTable.Players.Single(x => x.Name == GameTable.CurrentMovePlayerName).PlayerConnectionId).SendAsync("ReceiveFirstMove", "You make move");
                    await Clients.AllExcept(GameTable.Players.Single(x => x.Name == GameTable.CurrentMovePlayerName).PlayerConnectionId).SendAsync("ReceiveCurrentMovePlayer", $"{GameTable.Players.Single(x => x.Name == GameTable.CurrentMovePlayerName).Name} make move");
                }

                await GameInfoHelper.SendPlayersCards(GameTable, Clients);
                await SendInfoAboutOpponentsAsync();

                await Clients.All.SendAsync("ReceiveCardOnTableCount", GameTable.CardsOnTable.Count);
                await Clients.All.SendAsync("ReceiveDiscardCardsCount", GameTable.CountCardsForDiscard);
            }
        }

        public async Task SendInfoAboutOpponentsAsync()
        {
             await GameInfoHelper.SendInfoAboutOpponentsAsync(GameTable, Clients);
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

                await SendInfoAboutOpponentsAsync();
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
}