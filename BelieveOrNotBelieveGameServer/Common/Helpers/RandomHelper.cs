using BelieveOrNotBelieveGameServer.Common.Mappers;
using BelieveOrNotBelieveGameServer.Models;
using BelieveOrNotBelieveGameServer.Models.BotModels;

namespace BelieveOrNotBelieveGameServer.Common.Helpers;

public static class RandomHelper
{
    public static bool GenerateRandomBool()
    {
        var rnd = new Random();
        return (rnd.Next(0, 1)) switch
        {
            1 => true,
            _ => false
        };
    }

    public static int GenerateRandomInt(int maxValue, int minValue = 0)
    {
        var rnd = new Random();
        return rnd.Next(minValue, maxValue);
    }

    public static T GetRandomElmentFromList<T>(IEnumerable<T> orderedList)
    {
        var posibility = 10;
        var totalPosibility = orderedList.Count() * posibility;
        var half = totalPosibility / 2;
        var halfAndQuarter = half + totalPosibility / 4;
        var halfAndThird = half + totalPosibility / 3;
        var step = 1;

        var result = orderedList.FirstOrDefault();

        var coeficientOfNumber = GenerateRandomInt(totalPosibility, 0);

        if(coeficientOfNumber >= half && coeficientOfNumber < halfAndQuarter)
        {
            result = orderedList.Skip(step).FirstOrDefault();
        }
        else if (coeficientOfNumber >= halfAndQuarter && coeficientOfNumber < halfAndThird)
        {
            result = orderedList.Skip(step + 1).FirstOrDefault();
        }
        else 
        {
            result = orderedList.LastOrDefault();
        }

        return result;
    }

    public static List<PlayingCard> GetRandomCardsFromListByBotDificulty(List<PlayingCard> cards, BotDificulty botDificulty)
    {
        var botVisibility = BotMapper.MapBotVisibilityByDificulty(botDificulty);

        var numberOfCardsThatVisibleToBot = (int)(botVisibility * cards.Count);

        return GetRandomCardsFromList(cards, numberOfCardsThatVisibleToBot);
    }

    public static PlayingCard GetRandomCardFromList(List<PlayingCard> cards)
    {
        var index = GenerateRandomInt(cards.Count, 0);

        return cards[index];
    }

    public static List<PlayingCard> GetRandomCardsFromList(List<PlayingCard> cards, int numberOfCardsToReturn)
    {
        var playersCards = new HashSet<PlayingCard>();

        var rnd = new Random();

        for (int i = 0; i < numberOfCardsToReturn; i++)
        {
            int index = rnd.Next(cards.Count);
            var card = cards[index];
            if (!playersCards.Add(card))
            {
                i--;
            }
        }

        return playersCards.ToList();
    }
}
