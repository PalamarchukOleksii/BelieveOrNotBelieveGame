using BelieveOrNotBelieveGameServer.Common.Constants;
using BelieveOrNotBelieveGameServer.Common.Helpers;
using BelieveOrNotBelieveGameServer.Models.BotModels;
using BelieveOrNotBelieveGameServer.Services.Abstraction;

namespace BelieveOrNotBelieveGameServer.Services.BotServices;

public class BotNotFirstMoveService : IBotNotFirstMoveService
{
    private readonly IBotMakeMoveService _botMakeMoveService;

    public BotNotFirstMoveService(IBotMakeMoveService botMakeMoveService)
    {
        _botMakeMoveService = botMakeMoveService;
    }

    public BotResponse MakeNotFirstMove(BotInfo botInfo)
    {
        var myCards = botInfo.Bot.PlayersCards;
        var cardValue = botInfo.LastMove.CardValue!;
        var countOfCardsInLastMove = botInfo.LastMove.CardsId.Count;
                
        if (myCards.Any(c => c.Value == cardValue) 
            || botInfo.AllPlayersCards.Any(c => c.Value == cardValue)
            || botInfo.CardForDiscard.Any(c => c.Value == cardValue))
        {
            var mySameCardsCount = myCards.Where(c => c.Value == cardValue).Count();
            var allPlayersSameCardsCount = botInfo.AllPlayersCards.Where(c => c.Value == cardValue).Count();
            var cardForDiscardSameCardsCount = botInfo.AllPlayersCards.Where(c => c.Value == cardValue).Count();
            
            var lieIsMostPosibleAssume = mySameCardsCount + allPlayersSameCardsCount + countOfCardsInLastMove + cardForDiscardSameCardsCount > CardConstants.MaxCardsInOneMove;

            return DesideToAssumeOrMakeMove(botInfo, lieIsMostPosibleAssume);
        }
        else
        {
            return GenerateRandomAssume();
        }
    }

    private BotResponse DesideToAssumeOrMakeMove(
        BotInfo botInfo,         
        bool lieIsMostPosibleAssume)
    {
        var isAssuming = RandomHelper.GenerateRandomBool();

        if(isAssuming && lieIsMostPosibleAssume)
        {
            return new BotResponse(isBelieve: !lieIsMostPosibleAssume, isNotBelieve: lieIsMostPosibleAssume); 
        }
        else if(isAssuming)
        {
            return GenerateRandomAssume();
        }

        return _botMakeMoveService.MakeMove(botInfo, false);
    }

    private BotResponse GenerateRandomAssume()
    {
        var isBelieve = RandomHelper.GenerateRandomBool();
        return new BotResponse(isBelieve , !isBelieve);
    }
}
