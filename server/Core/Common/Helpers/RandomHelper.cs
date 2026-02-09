using GameCore.Common.Mappers;
using GameCore.Enums;
using GameCore.Models;

namespace GameCore.Common.Helpers;

public static class RandomHelper
{
    private static readonly Random _random = new();

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

        var coeficientOfNumber = GenerateRandomInt(totalPosibility);

        if (coeficientOfNumber >= half && coeficientOfNumber < halfAndQuarter)
            result = orderedList.Skip(step).FirstOrDefault();
        else if (coeficientOfNumber >= halfAndQuarter && coeficientOfNumber < halfAndThird)
            result = orderedList.Skip(step + 1).FirstOrDefault();
        else
            result = orderedList.LastOrDefault();

        return result;
    }

    public static List<Card> GetRandomCardsFromListByBotDificulty(List<Card> cards, BotDificulty botDificulty)
    {
        var botVisibility = BotMapper.MapBotVisibilityByDificulty(botDificulty);

        var numberOfCardsThatVisibleToBot = (int)(botVisibility * cards.Count);

        return GetRandomCardsFromList(cards, numberOfCardsThatVisibleToBot);
    }

    public static Card GetRandomCardFromList(List<Card> cards)
    {
        var index = GenerateRandomInt(cards.Count);

        return cards[index];
    }

    public static List<Card> GetRandomCardsFromList(List<Card> cards, int numberOfCardsToReturn)
    {
        var playersCards = new HashSet<Card>();

        while (playersCards.Count < numberOfCardsToReturn)
        {
            var index = _random.Next(cards.Count);
            playersCards.Add(cards[index]);
        }

        return playersCards.ToList();
    }
}