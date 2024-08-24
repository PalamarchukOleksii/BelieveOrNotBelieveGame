﻿using BelieveOrNotBelieveGameServer.Common.Constants;
using BelieveOrNotBelieveGameServer.Common.Helpers;
using BelieveOrNotBelieveGameServer.Models;
using BelieveOrNotBelieveGameServer.Models.BotModels;
using BelieveOrNotBelieveGameServer.Services.Abstraction;
using Hellang.Middleware.ProblemDetails;

namespace BelieveOrNotBelieveGameServer.Services.BotServices;

public class BotService : IBotService
{
    private readonly IBotFirstMoveService _botFirstMoveService;
    private readonly IBotNotFirstMoveService _botNotFirstMoveService;

    public BotService(BotFirstMoveService botFirstMoveService, IBotNotFirstMoveService botNotFirstMoveService)
    {
        _botNotFirstMoveService = botNotFirstMoveService;
        _botFirstMoveService = botFirstMoveService;
    }

    private decimal probapility = 0;

    public BotResponse MakeMove(GameTable gameTable)
    {
        var previousPlayer = gameTable.CurrentMovePlayer;
        var nextPlayer = gameTable.NextMovePlayer;
        var bot = gameTable.CurrentMovePlayer;

        if (!bot.IsBot || bot.BotDifficulty == BotDificulty.ItIsNotABot)
        {
            throw new ProblemDetailsException(
                StatusCodes.Status400BadRequest,
                $"Error! Bot can't move instead of player {bot.Name}");
        }

        (var otherPlayers, var cardForDiscard) = ConfigureBotInfo(gameTable);

        var isNotFirstMove = gameTable.CardsOnTable.Any();

        if (isNotFirstMove && gameTable.Move is null)
        {
            throw new ProblemDetailsException(StatusCodes.Status400BadRequest, "Move can't be null");
        }

        if (isNotFirstMove)
        {
            return _botNotFirstMoveService.MakeNotFirstMove(bot, otherPlayers, cardForDiscard, gameTable);
        }
        else
        {
            return _botFirstMoveService.MakeFirstMove(bot, otherPlayers, cardForDiscard, gameTable);
        }
    }

    private (List<Player> otherPlayers, List<PlayingCard> cardForDiscard) ConfigureBotInfo(GameTable gameTable)
    {
        var botDifficulty = gameTable.CurrentMovePlayer.BotDifficulty;

        var players = gameTable.Players
            .Where(p => p.Name != gameTable.CurrentMovePlayer.Name)
            .Select(p =>
            {
                var playersCards = RandomHelper.GetRandomCardsFromListByBotDificulty(p.PlayersCards, botDifficulty);

                return new Player
                {
                    PlayerConnectionId = p.PlayerConnectionId,
                    Name = p.Name,
                    StartGame = p.StartGame,
                    BotDifficulty = p.BotDifficulty,
                    IsBot = p.IsBot,
                    PlayersCards = playersCards.ToList(),
                };
            }).ToList()
            ?? new List<Player>();

        var cardForDiscard = RandomHelper.GetRandomCardsFromListByBotDificulty(gameTable.CardsForDiscard, botDifficulty);

        return (players, cardForDiscard);
    }    
}