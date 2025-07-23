using Domain.Constants;

namespace Domain.Common.Mappers
{
    public static class CardsDeckMapper
    {
        public static string[] MapCardsValueByNumOfCards(int numofCards)
        {
            return numofCards switch
            {
                24 => CardConstants.Values24,
                36 => CardConstants.Values36,
                52 => CardConstants.Values52,
                _ => []
            };
        }
    }
}
