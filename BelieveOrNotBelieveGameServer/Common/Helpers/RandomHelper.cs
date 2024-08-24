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

    public static List<PlayingCard> GetRandomCardsFromListByBotDificulty(List<PlayingCard> cards, BotDificulty botDificulty)
    {
        var botVisibility = BotMapper.MapBotVisibilityByDificulty(botDificulty);

        var numberOfCardsThatVisibleToBot = (int)(botVisibility * cards.Count);

        return GetRandomCardsFromList(cards, numberOfCardsThatVisibleToBot);
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
