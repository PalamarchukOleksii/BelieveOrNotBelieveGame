
using Application.GameTable.Commands.CreateGameCommand;
using Application.GameTable.Commands.EndGameCommand;
using Application.GameTable.Commands.JoinGameCommand;
using Application.GameTable.Commands.LeaveGameCommand;
using Application.GameTable.Commands.MakeAssumeCommand;
using Application.GameTable.Commands.MakeMoveCommand;
using Application.GameTable.Commands.StartGameCommand;
using Application.GameTable.Queries.GetGameNameByConnectionIdQuery;
using Application.GameTable.Queries.GetGameStateQuery;
using Application.GameTable.Queries.GetInfoAboutOpponentsQuery;
using Application.GameTable.Queries.GetPlayerCardsQuery;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Presentation.Dtos;

namespace Presentation.Hubs;

public class GameHub : Hub
{
    private readonly IMediator _mediator;

    public GameHub(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task OnConnectedAsync()
    {
        await Clients.Caller.SendAsync("ReceiveConnectionId", Context.ConnectionId);

        await base.OnConnectedAsync();
    }

    public async Task CreateGame(CreateGameDto createGameInfo)
    {
        var response = await _mediator.Send(new CreateGameCommandRequest
        {
            GameName = createGameInfo.GameName,
            NumOfCards = createGameInfo.NumOfCards,
            MaxNumOfPlayers = createGameInfo.MaxNumOfPlayers,
            AddBot = createGameInfo.AddBot
        });

        if (response.Success)
        {
            await Clients.All.SendAsync("RecieveNewGameCreated", response.Message, response.GameTable);

            await JoinGameTable(createGameInfo.CreatorName, createGameInfo.GameName);
        }
        else
        {
            await Clients.Caller.SendAsync("RecieveNewGameNotCreated", response.Message);
        }
    }

    public async Task JoinGameTable(string username, string gameName)
    {
        var response = await _mediator.Send(new JoinGameCommandRequest
        {
            Username = username,
            CallerConnectionId = Context.ConnectionId,
            GameName = gameName
        });

        if (response.Success)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, gameName);

            await Clients.GroupExcept(gameName, Context.ConnectionId).SendAsync("ReceiveJoin", response.Message);

            await SendInfoAboutOpponents(gameName);
        }
        else
        {
            await Clients.Caller.SendAsync("ReceiveNotJoin", response.Message);
        }
    }

    public async Task StartGame(string gameName)
    {
        var response = await _mediator.Send(new StartGameCommandRequest
        {
            CallerConnectionId = Context.ConnectionId,
            GameName = gameName
        });

        if (response.Success)
        {
            await Clients.Group(gameName).SendAsync("RecieveGameStarted", response.Message);

            await SendInfoAboutOpponents(gameName);
            await SendPlayersCards(gameName);
        }
        else
        {
            await Clients.Group(gameName).SendAsync("RecieveGameNotStarted", response.Message);
        }
    }

    public async Task MakeMove(MakeMoveDto makeMoveDto)
    {
        var response = await _mediator.Send(new MakeMoveCommandRequest
        {
            CallerConnectionId = Context.ConnectionId,
            GameName = makeMoveDto.GameName,
            CardsId = makeMoveDto.CardsId,
            CardsValue = makeMoveDto.CardsValue
        });

        if (response.Success)
        {
            await Clients.Group(makeMoveDto.GameName).SendAsync("RecieveMove", response.Message);

            await SendGameState(makeMoveDto.GameName);
            await SendInfoAboutOpponents(makeMoveDto.GameName);
            await SendPlayersCards(makeMoveDto.GameName);
        }
        else
        {
            await Clients.Caller.SendAsync("RecieveNotMove", response.Message);
        }
    }

    public async Task MakeAssume(MakeAssumeDto makeAssumeDto)
    {
        var response = await _mediator.Send(new MakeAssumeCommandRequest
        {
            CallerConnectionId = Context.ConnectionId,
            GameName = makeAssumeDto.GameName,
            IBelieve = makeAssumeDto.IBelieve
        });

        if (response.Success)
        {
            await Clients.Group(makeAssumeDto.GameName).SendAsync("RecieveNotAssume", response.Message);

            if (response.EndGame)
            {
                await SendEndGame(makeAssumeDto.GameName);
            }
            else
            {
                await SendGameState(makeAssumeDto.GameName);
                await SendInfoAboutOpponents(makeAssumeDto.GameName);
                await SendPlayersCards(makeAssumeDto.GameName);
            }
        }
        else
        {
            await Clients.Caller.SendAsync("RecieveNotAssume", response.Message);
        }
    }

    public async Task SendInfoAboutOpponents(string gameName)
    {
        var response = await _mediator.Send(new GetInfoAboutOpponentsQueryRequest
        {
            GameName = gameName
        });

        if (response.Success)
        {
            foreach (var playerConnectionId in response.PlayersConnectionIds)
                await Clients.Client(playerConnectionId)
                    .SendAsync("ReceiveOpponentsInfo",
                        response.OpponentInfo.Where(x => x.PlayerConnectionId != playerConnectionId));

            foreach (var playerWhoWinConnectionId in response.PlayersWhoWinConnectionIds)
                await Clients.Client(playerWhoWinConnectionId)
                    .SendAsync("ReceiveOpponentsInfo", response.OpponentInfo);
        }
        else
        {
            await Clients.Caller.SendAsync("NotReceiveOpponentsInfo", response.Message);
        }
    }

    public async Task SendPlayersCards(string gameName)
    {
        var response = await _mediator.Send(new GetPlayerCardsQueryRequest { GameName = gameName });

        if (response.Success)
            foreach (var player in response.PlayersCards)
                await Clients.Client(player.PlayerConnectionId).SendAsync("ReceiveCard", player.Cards);
        else
            await Clients.Caller.SendAsync("NotReceiveCard", response.Message);
    }

    public async Task SendGameState(string gameName)
    {
        var response = await _mediator.Send(new GetGameStateQueryRequest { GameName = gameName });

        if (response.Success)
        {
            if (response.CurrentPlayerCanMakeMove && response.CurrentPlayerCanMakeAssume)
            {
                await Clients.Client(response.CurrentMovePlayerConnectionId)
                    .SendAsync("ReceiveNextMoveAssume", "You make assume or move");
                await Clients.GroupExcept(gameName, response.CurrentMovePlayerConnectionId)
                    .SendAsync("ReceiveCurrentMovePlayer", $"{response.CurrentMovePlayerName} make assume or move");
            }
            else if (response.CurrentPlayerCanMakeAssume)
            {
                await Clients.Client(response.CurrentMovePlayerConnectionId)
                    .SendAsync("ReceiveNextAssume", "You make assume");
                await Clients.GroupExcept(gameName, response.CurrentMovePlayerConnectionId)
                    .SendAsync("ReceiveCurrentMovePlayer", $"{response.CurrentMovePlayerName} make assume");
            }
            else
            {
                await Clients.Client(response.CurrentMovePlayerConnectionId)
                    .SendAsync("ReceiveNextMove", "You make move");
                await Clients.GroupExcept(gameName, response.CurrentMovePlayerConnectionId)
                    .SendAsync("ReceiveCurrentMovePlayer", $"{response.CurrentMovePlayerName} make move");
            }

            await Clients.Group(gameName).SendAsync("ReceiveCardOnTableCount", response.CardsOnTableCount);
            await Clients.Group(gameName).SendAsync("ReceiveMakeMoveValues", response.MakeMoveValue);
        }
        else
        {
            await Clients.Caller.SendAsync("NotReceiveCard", response.Message);
        }
    }

    public async Task SendEndGame(string gameName)
    {
        var response = await _mediator.Send(new EndGameCommandRequest { GameName = gameName });
        if (response.Success)
        {
            await Clients.Group(gameName).SendAsync("RecieveEndGame", response.Message);

            await SendInfoAboutOpponents(gameName);
            await SendPlayersCards(gameName);
        }
        else
        {
            await Clients.Group(gameName).SendAsync("RecieveNotEndGame", response.Message);
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var gameNameResponse = await _mediator.Send(new GetGameNameByConnectionIdQueryRequest
            { CallerConnectionId = Context.ConnectionId });
        if (gameNameResponse.Success)
        {
            var leaveResponse = await _mediator.Send(new LeaveGameCommandRequest
            {
                CallerConnectionId = Context.ConnectionId,
                GameName = gameNameResponse.GameName
            });

            if (leaveResponse.Success)
            {
                await Clients.Group(gameNameResponse.GameName).SendAsync("RecievePlayerLeave", leaveResponse.Message);
                if (leaveResponse.EndGame) await SendEndGame(gameNameResponse.GameName);
            }
        }

        await base.OnDisconnectedAsync(exception);
    }
}