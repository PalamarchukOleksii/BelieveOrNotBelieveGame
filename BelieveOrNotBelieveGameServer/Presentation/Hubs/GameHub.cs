using Application.GameTable.Commands.CreateGameCommand;
using Application.GameTable.Commands.JoinGameCommand;
using Application.GameTable.Commands.MakeMoveCommand;
using Application.GameTable.Commands.StartGameCommand;
using Application.GameTable.Queries.GetInfoAboutOpponentsQuery;
using Application.GameTable.Queries.GetPlayerCardsQuery;
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
            JoinGameCommandResponse response = await _mediator.Send(new JoinGameCommandRequest
            {
                Username = username,
                CallerConnectionId = Context.ConnectionId,
                GameName = gameName
            });

            if (response.Success)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, gameName);

                await Clients.GroupExcept(gameName, Context.ConnectionId).SendAsync("ReceiveJoin", response.Message);

                await GetInfoAboutOpponents(gameName);
            }
            else
            {
                await Clients.Caller.SendAsync("ReceiveNotJoin", response.Message);
            }
        }

        public async Task StartGame(string gameName)
        {
            StartGameCommandResponse response = await _mediator.Send(new StartGameCommandRequest
            {
                CallerConnectionId = Context.ConnectionId,
                GameName = gameName
            });

            if (response.Result == "GameStarted")
            {
                await Clients.Group(gameName).SendAsync("RecieveGameStarted", response.Message);
                await Clients.Client(response.CurrentMovePlayerConnectionId).SendAsync("ReceiveStartMove", "You start the game");

                await Clients.GroupExcept(gameName, response.CurrentMovePlayerConnectionId)
                    .SendAsync("ReceiveCurrentMovePlayer", $"{response.CurrentMovePlayerName} make move");
                await Clients.Group(gameName).SendAsync("ReceiveMakeMoveValues", response.MakeMoveValue);

                await GetInfoAboutOpponents(gameName);
                await GetPlayersCards(gameName);
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

        public async Task MakeMove(MakeMoveDto makeMoveDto)
        {
            MakeMoveCommandResponse response = await _mediator.Send(new MakeMoveCommandRequest
            {
                CallerConnectionId = Context.ConnectionId,
                GameName = makeMoveDto.GameName,
                CardsId = makeMoveDto.CardsId,
                CardsValue = makeMoveDto.CardsValue
            });

            if (response.Success)
            {
                await Clients.Group(makeMoveDto.GameName).SendAsync("RecieveMove", response.Message);

                if (response.CurrentPlayerCanMakeMove)
                {
                    await Clients.Client(response.CurrentMovePlayerConnectionId)
                        .SendAsync("ReceiveNextMoveAssume", "You make assume or move");
                    await Clients.GroupExcept(makeMoveDto.GameName, response.CurrentMovePlayerConnectionId)
                        .SendAsync("ReceiveCurrentMovePlayer", $"{response.CurrentMovePlayerName} make assume or move");
                }
                else
                {
                    await Clients.Client(response.CurrentMovePlayerConnectionId).SendAsync("ReceiveNextMoveAssume", "You make assume");
                    await Clients.GroupExcept(makeMoveDto.GameName, response.CurrentMovePlayerConnectionId)
                        .SendAsync("ReceiveCurrentMovePlayer", $"{response.CurrentMovePlayerName} make assume");
                }

                await Clients.Group(makeMoveDto.GameName).SendAsync("ReceiveCardOnTableCount", response.CardsOnTableCount);

                await GetPlayersCards(makeMoveDto.GameName);
            }
            else
            {
                await Clients.Caller.SendAsync("RecieveNotMove", response.Message);
            }
        }

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

        public async Task GetInfoAboutOpponents(string gameName)
        {
            GetInfoAboutOpponentsQueryResponse response = await _mediator.Send(new GetInfoAboutOpponentsQueryRequest
            {
                GameName = gameName
            });

            if (response.Success)
            {
                foreach (var playerConnectionId in response.PlayersConnectionIds)
                {
                    await Clients.Client(playerConnectionId)
                        .SendAsync("ReceiveOpponentsInfo", response.OpponentInfo.Where(x => x.PlayerConnectionId != playerConnectionId));
                }

                foreach (var playerWhoWinConnectionId in response.PlayersWhoWinConnectionIds)
                {
                    await Clients.Client(playerWhoWinConnectionId)
                        .SendAsync("ReceiveOpponentsInfo", response.OpponentInfo);
                }
            }
            else
            {
                await Clients.Caller.SendAsync("NotReceiveOpponentsInfo", "Error on server");

            }
        }

        public async Task GetPlayersCards(string gameName)
        {
            GetPlayerCardsQueryResponse response = await _mediator.Send(new GetPlayerCardsQueryRequest
            {
                GameName = gameName
            });

            if (response.Success)
            {
                foreach (var player in response.PlayersCards)
                {
                    await Clients.Client(player.PlayerConnectionId).SendAsync("ReceiveCard", player.Cards);
                }
            }
            else
            {
                await Clients.Caller.SendAsync("NotReceiveCard", "Error on server");
            }
        }

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