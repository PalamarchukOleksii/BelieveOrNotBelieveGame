using BelieveOrNotBelieveGameServer.Common.Constants;
using BelieveOrNotBelieveGameServer.Models;

namespace BelieveOrNotBelieveGameServer.Common.Helpers;

public static class CardsDeckHelper
{
    public static void ShuffleCards(CardsDeck cardsDeck)
    {
        Random rnd = new Random();
        for(int i = 0; i < cardsDeck.NumOfCard; i++)
        {
            int j = rnd.Next(0,cardsDeck.NumOfCard -1);
            (cardsDeck.Cards[i], cardsDeck.Cards[j]) = (cardsDeck.Cards[j], cardsDeck.Cards[i]);
        }
    }

    public static void GenerateCardsDeck(CardsDeck cardsDeck, string[] values)
    {
        int count = 1;

            foreach (var suit in CardConstants.Suits)
            {
                foreach (var value in values)
                {
                    cardsDeck.Cards.Add(new PlayingCard { Id = count++, CardSuit = suit, CardValue = value });
                }
            }
    }

    public static void GiveCardsToPlayers(CardsDeck cardsDeck, List<Player> players)
    {
        int numCardForOnePlayer = cardsDeck.NumOfCard / players.Count;
        int c = 0;
        for (int i = 0; i < numCardForOnePlayer / 2; i++)
        {
            for (int j = 0; j < players.Count; j++)
            {
                players[j].PlayersCards.Add(cardsDeck.Cards[c++]);
                players[j].PlayersCards.Add(cardsDeck.Cards[c++]);
            }
        }
    }
}
