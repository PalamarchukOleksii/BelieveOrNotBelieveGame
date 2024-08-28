using BelieveOrNotBelieveGameServer.Common.Helpers;
using BelieveOrNotBelieveGameServer.Models;
using BelieveOrNotBelieveGameServer.Models.BotModels;
using BelieveOrNotBelieveGameServer.Services.Abstraction;

namespace BelieveOrNotBelieveGameServer.Services.BotServices;

public class BotMakeMoveService : IBotMakeMoveService
{
    public BotResponse MakeMove(BotInfo botInfo, bool isFirstMove)
    {
        var myCards = botInfo.Bot.PlayersCards;
        var cardValue = isFirstMove 
            ? GetCardsValue(myCards)
            : botInfo.LastMove.CardValue!;
        
        var isToTellTruth = RandomHelper.GenerateRandomBool();
        var cardsForMove = new List<PlayingCard>();

        if(isToTellTruth && myCards.Any(c => c.Value == cardValue) )
        {
            cardsForMove = myCards.Where(x => x.Value == cardValue).ToList();
        }
        else
        {
            var numberOfCardsForMove = RandomHelper.GetRandomElmentFromList(new List<int>{1,2,3,4});

            cardsForMove = RandomHelper.GetRandomCardsFromList(myCards, numberOfCardsForMove);
        }

        return new BotResponse
            (
                new Move
                (
                    botInfo.Bot.Name,
                    cardValue,
                    cardsForMove.Select(x => x.Id).ToList()
                )
            );
    }

    private string GetCardsValue(List<PlayingCard> myCards)
    {
        var allValues = myCards
            .GroupBy(c => c.Value)
            .OrderByDescending(x => x.Count())
            .Select(x => x.Key)
            .ToList();

        var numberOfCardsForMove = RandomHelper.GetRandomElmentFromList(allValues);

        return numberOfCardsForMove ?? allValues.First();
    }
}
