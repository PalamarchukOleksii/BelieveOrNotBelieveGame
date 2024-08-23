using BelieveOrNotBelieveGameServer.Common.Constants;
using BelieveOrNotBelieveGameServer.Common.Helpers;

namespace BelieveOrNotBelieveGameServer.Models
{
    public class CardsDeck
    {
        public int NumOfCard { get; set; }
        public List<PlayingCard> Cards { get; set; }

        public CardsDeck(int numOfCard)
        {
            NumOfCard = numOfCard;
            Cards = new List<PlayingCard>();
            switch (numOfCard)
            {
                case 24:
                    CardsDeckHelper.GenerateCardsDeck(this, CardConstants.Values24);
                    break;
                case 36:
                    CardsDeckHelper.GenerateCardsDeck(this, CardConstants.Values36);
                    break;
                case 52:
                    CardsDeckHelper.GenerateCardsDeck(this, CardConstants.Values52);
                    break;
                default:
                    throw new ArgumentException("Unsupported number of cards");
            }
        }
    }
}
