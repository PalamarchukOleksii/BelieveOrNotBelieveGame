using Domain.Common.Mappers;
using Domain.Constants;

namespace Domain.Models.GameModels
{
    public class CardsDeck
    {
        public int NumOfCards { get; private set; }
        public string[] Values { get; private set; }
        public List<PlayingCard> Cards { get; private set; }

        public CardsDeck(int numOfCard)
        {
            NumOfCards = numOfCard;
            Cards = new List<PlayingCard>();
            Values = CardsDeckMapper.MapCardsValueByNumOfCards(numOfCard);
        }

        public void ShuffleCardsDeck()
        {
            Random rnd = new Random();
            for (int i = 0; i < NumOfCards; i++)
            {
                int j = rnd.Next(0, NumOfCards - 1);
                (Cards[i], Cards[j]) = (Cards[j], Cards[i]);
            }
        }

        public void GenerateCardsDeck()
        {
            int count = 0;

            foreach (var suit in CardConstants.Suits)
            {
                foreach (var value in Values)
                {
                    Cards.Add(new PlayingCard { Id = count++, Suit = suit, Value = value });
                }
            }
        }

        public void GiveCardsToPlayers(List<Player> players)
        {
            int numCardForOnePlayer = NumOfCards / players.Count;
            int c = 0;
            for (int i = 0; i < numCardForOnePlayer / 2; i++)
            {
                for (int j = 0; j < players.Count; j++)
                {
                    players[j].PlayersCards.Add(Cards[c++]);
                    players[j].PlayersCards.Add(Cards[c++]);
                }
            }
        }
    }
}
