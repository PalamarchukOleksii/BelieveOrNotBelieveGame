using GameCore.Abstractions;
using GameCore.Common.Helpers;
using GameCore.Models;

namespace GameCore.Services;

public class BotMakeMoveService : IBotMakeMoveService
{
    public BotResponse MakeMove(BotInfo botInfo, bool isFirstMove)
    {
        var myCards = botInfo.Bot.Cards;
        var cardValue = isFirstMove
            ? GetCardsValue(myCards.ToList())
            : botInfo.LastMove.CardValue!;

        var isToTellTruth = RandomHelper.GenerateRandomBool();
        var cardsForMove = new List<Card>();

        if (isToTellTruth && myCards.Any(c => c.Value == cardValue))
        {
            cardsForMove = myCards.Where(x => x.Value == cardValue).ToList();
        }
        else
        {
            var numberOfCardsForMove = RandomHelper.GetRandomElmentFromList(new List<int> { 1, 2, 3, 4 });

            cardsForMove = RandomHelper.GetRandomCardsFromList(myCards.ToList(), numberOfCardsForMove);
        }

        return new BotResponse
        (
            new Move
            (
                cardValue,
                cardsForMove.Select(x => x.Id).ToList()
            )
        );
    }

    private string GetCardsValue(List<Card> myCards)
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