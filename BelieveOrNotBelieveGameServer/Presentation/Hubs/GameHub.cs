using Application.GameTable.Commands.CreateGameCommand;
using Application.GameTable.Commands.JoinGameCommand;
using Application.GameTable.Commands.StartGameCommand;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Presentation.Dtos;

namespace Presentation.Hubs
{
    public class GameHub : Hub
    {
        private readonly IMediator _mediator;

        public GameHub(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task CreateGame(CreateGameDto createGameDto)
        {
            CreateGameCommandResponse response = await _mediator.Send(new CreateGameCommandRequest
            {
                GameName = createGameDto.GameName,
                NumOfCards = createGameDto.NumOfCards,
                AddBot = createGameDto.AddBot,
                MaxNumOfPlayers = createGameDto.MaxNumOfPlayers,
            });

            if (response.Successs)
            {
                await Clients.All.SendAsync("RecieveNewGameCreated", response.Message);

                await JoinGameTable(createGameDto.CreatorName, createGameDto.GameName);
            }
            else
            {
                await Clients.Caller.SendAsync("RecieveNewGameNotCreated", response.Message);
            }
        }

        public async Task JoinGameTable(string username, string gameName)
        {
            JoinGameCommandResponse response = await _mediator.Send(new JoinGameCommandRequest { Username = username, CallerConnectionId = Context.ConnectionId, GameName = gameName });

            if (response.Success)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, gameName);
                await Clients.Group(gameName).SendAsync("ReceiveJoin", response.Message);

                //await SendInfoAboutOpponents();
            }
            else
            {
                await Clients.Caller.SendAsync("ReceiveNotJoin", response.Message);
            }
        }

        public async Task StartGame(string gameName)
        {
            StartGameCommandResponse response = await _mediator.Send(new StartGameCommandRequest { CallerConnectionId = Context.ConnectionId, GameName = gameName });

            if (response.Result == "GameStarted")
            {
                await Clients.Group(gameName).SendAsync("RecieveGameStarted", response.Message);

                //await SendPlayersCards();
                //await SendInfoAboutOpponents();

                await Clients.Client(response.CurrentMovePlayerConnectionId).SendAsync("ReceiveStartMove", "You start the game");
                await Clients.AllExcept(response.CurrentMovePlayerConnectionId).SendAsync("ReceiveCurrentMovePlayer", $"{response.CurrentMovePlayerName} make move");
                await Clients.Group(gameName).SendAsync("ReceiveMakeMoveValues", response.MakeMoveValue);
            }
            else if (response.Result == "GameNotStarted")
            {
                await Clients.Group(gameName).SendAsync("RecieveGameNotStarted", response.Message);
            }
            else
            {
                await Clients.Group(gameName).SendAsync("RecieveWaitingForOtherPlayers", response.Message);
            }
        }

        //public async Task MakeMove(string cardsValue, string cardsId)
        //{
        //    Player pl = GameTable.Players.Single(x => x.PlayerConnectionId == Context.ConnectionId);
        //    if (pl.PlayersCards.Count > 0 && pl.Name == GameTable.CurrentMovePlayer.Name && PlayerHelper.CheckIfPlayerHaveSomeCards(pl, cardsId))
        //    {
        //        string playerName = pl.Name;
        //        Move playerMove = new Move(playerName, cardsValue, cardsId);

        //        GameTable.MakeMove(playerMove);

        //        await Clients.All.SendAsync("RecieveMove", $"{playerName} throw {playerMove.CardsId.Count} {playerMove.CardValue}");

        //        Player nextPl = GameTable.Players.Single(x => x.Name == GameTable.CurrentMovePlayer.Name);

        //        if (nextPl.PlayersCards.Count == 0)
        //        {
        //            await Clients
        //                .Client(nextPl.PlayerConnectionId)
        //                .SendAsync("ReceiveNextAssume", "You make assume");

        //            await Clients.AllExcept(
        //                GameTable.Players
        //                .Single(x => x.Name == GameTable.CurrentMovePlayer.Name)
        //                .PlayerConnectionId)
        //                .SendAsync("ReceiveCurrentMovePlayer", $"{GameTable.Players.Single(x => x.Name == GameTable.CurrentMovePlayer.Name).Name} make move");
        //        }
        //        else
        //        {
        //            await Clients
        //                .Client(nextPl.PlayerConnectionId)
        //                .SendAsync("ReceiveNextMoveAssume", "You make assume or move");

        //            await Clients
        //                .AllExcept(
        //                GameTable.Players
        //                .Single(x => x.Name == GameTable.CurrentMovePlayer.Name)
        //                .PlayerConnectionId)
        //                .SendAsync("ReceiveCurrentMovePlayer", $"{GameTable.Players.Single(x => x.Name == GameTable.CurrentMovePlayer.Name).Name} make assume or move");
        //        }

        //        await Clients.All.SendAsync("ReceiveCardOnTableCount", GameTable.CardsOnTable.Count);

        //        await SendPlayersCards();
        //        await SendInfoAboutOpponents();
        //    }
        //    else if (pl.PlayersCards.Count == 0)
        //    {
        //        await Clients.Caller.SendAsync("RecieveNotMove", "You can only make an assume");
        //    }
        //}

        //public async Task MakeAssume(bool iBelieve)
        //{
        //    if (GameTable.Players.Single(x => x.PlayerConnectionId == Context.ConnectionId).Name == GameTable.CurrentMovePlayer.Name)
        //    {
        //        (int ResultId, string ClientMessage) = GameTable.MakeAssume(iBelieve);

        //        if (ResultId == 5)
        //        {
        //            GameTable.GameStarted = false;
        //            await Clients.All.SendAsync("ReceiveGameOver", ClientMessage);
        //            GameTable.EndGame();
        //        }
        //        else
        //        {
        //            await Clients.All.SendAsync("ReceiveAssume", ClientMessage);
        //            await Clients.Client(
        //                GameTable.Players.
        //                Single(x => x.Name == GameTable.CurrentMovePlayer.Name)
        //                .PlayerConnectionId)
        //                .SendAsync("ReceiveFirstMove", "You make move");
        //            await Clients.AllExcept(
        //                GameTable.Players
        //                .Single(x => x.Name == GameTable.CurrentMovePlayer.Name)
        //                .PlayerConnectionId)
        //                .SendAsync("ReceiveCurrentMovePlayer", $"{GameTable.Players.Single(x => x.Name == GameTable.CurrentMovePlayer.Name).Name} make move");
        //        }

        //        await SendPlayersCards();
        //        await SendInfoAboutOpponents();

        //        await Clients.All.SendAsync("ReceiveCardOnTableCount", GameTable.CardsOnTable.Count);
        //        await Clients.All.SendAsync("ReceiveDiscardCardsCount", GameTable.CountCardsForDiscard);
        //    }
        //}

        //public async Task SendInfoAboutOpponents()
        //{
        //    List<OpponentInfoDto> opponentsInfo = gameTable.Players
        //        .Select(p => new OpponentInfoDto
        //        {
        //            Name = p.Name,
        //            CardCount = p.PlayersCards.Count
        //        })
        //        .ToList();

        //    foreach (var player in gameTable.Players)
        //    {
        //        await clients.Client(player.PlayerConnectionId).SendAsync("ReceiveOpponentsInfo", opponentsInfo.Where(x => x.Name != player.Name));
        //    }

        //    foreach (var playerWhoWin in gameTable.PlayersWhoWin)
        //    {
        //        await clients.Client(playerWhoWin.PlayerConnectionId).SendAsync("ReceiveOpponentsInfo", opponentsInfo.Where(x => x.Name != playerWhoWin.Name));
        //    }
        //}

        //public static async Task SendPlayersCards()
        //{
        //    for (int i = 0; i < gameTable.Players.Count; i++)
        //    {
        //        await clients.Client(gameTable.Players[i].PlayerConnectionId).SendAsync("ReceiveCard", gameTable.Players[i].PlayersCards);
        //    }
        //}

        //public override async Task OnDisconnectedAsync(Exception? exception)
        //{
        //    if (GameTable.Players.Exists(x => x.PlayerConnectionId == Context.ConnectionId))
        //    {
        //        if (GameTable.GameStarted)
        //        {
        //            GameTable.GameStarted = false;
        //            await Clients.All.SendAsync("ReceiveGameOver", $"Game over, {GameTable.Players.Single(x => x.PlayerConnectionId == Context.ConnectionId).Name} lose");
        //        }

        //        GameTable.Players.Remove(GameTable.Players.Single(x => x.PlayerConnectionId == Context.ConnectionId));
        //        GameTable.EndGame();

        //        await SendInfoAboutOpponents();
        //    }
        //    await base.OnDisconnectedAsync(exception);
        //}
    }
}