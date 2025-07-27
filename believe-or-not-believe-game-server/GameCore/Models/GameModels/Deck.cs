using GameCore.Common.Mappers;
using GameCore.Constants;

namespace GameCore.Models.GameModels;

public class Deck
{
    public Deck(int numOfCard)
    {
        NumOfCards = numOfCard;
        Cards = new List<Card>();
        Values = CardsDeckMapper.MapCardsValueByNumOfCards(numOfCard);
    }

    public int NumOfCards { get; }
    public string[] Values { get; }
    public List<Card> Cards { get; }

    public void ShuffleCardsDeck()
    {
        var rnd = new Random();
        for (var i = 0; i < NumOfCards; i++)
        {
            var j = rnd.Next(0, NumOfCards - 1);
            (Cards[i], Cards[j]) = (Cards[j], Cards[i]);
        }
    }

    public void GenerateCardsDeck()
    {
        var count = 0;

        foreach (var suit in CardConstants.Suits)
        foreach (var value in Values)
            Cards.Add(new Card { Id = count++, Suit = suit, Value = value });
    }

    public void GiveCardsToPlayers(List<Player> players)
    {
        var numCardForOnePlayer = NumOfCards / players.Count;
        var c = 0;
        for (var i = 0; i < numCardForOnePlayer / 2; i++)
        for (var j = 0; j < players.Count; j++)
        {
            players[j].PlayersCards.Add(Cards[c++]);
            players[j].PlayersCards.Add(Cards[c++]);
        }
    }
}