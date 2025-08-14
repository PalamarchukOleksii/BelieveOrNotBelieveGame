using GameCore.Constants;

namespace GameCore.Common.Mappers;

public static class CardsDeckMapper
{
    private static readonly Dictionary<int, string[]> ValueMap = new()
    {
        [24] = CardConstants.Values24,
        [36] = CardConstants.Values36,
        [52] = CardConstants.Values52
    };

    public static string[] MapCardsValueByNumOfCards(int numOfCards)
    {
        if (!ValueMap.TryGetValue(numOfCards, out var values))
            throw new ArgumentOutOfRangeException(nameof(numOfCards), $"Unsupported deck size: {numOfCards}");

        return values;
    }

    public static bool IsSupportedDeckSize(int numOfCards) => ValueMap.ContainsKey(numOfCards);

    public static IEnumerable<int> GetSupportedDeckSizes() => ValueMap.Keys;
}