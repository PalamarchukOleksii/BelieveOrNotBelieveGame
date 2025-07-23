using Domain.Common.Mappers;
using Domain.Enums;
using Domain.Models.GameModels;

namespace Domain.Common.Helpers;

public static class RandomHelper
{
    private static readonly Random _random = new Random();

    public static bool GenerateRandomBool()
    {
        return _random.Next(2) == 1;
    }

    public static int GenerateRandomInt(int maxValue, int minValue = 0)
    {
        return _random.Next(minValue, maxValue + 1);
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

        if (coeficientOfNumber >= half && coeficientOfNumber < halfAndQuarter)
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

        while (playersCards.Count < numberOfCardsToReturn)
        {
            int index = _random.Next(cards.Count);
            playersCards.Add(cards[index]);
        }

        return playersCards.ToList();
    }
}
