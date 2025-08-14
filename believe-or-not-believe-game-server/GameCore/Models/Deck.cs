using GameCore.Common.Mappers;
using GameCore.Constants;

namespace GameCore.Models;

public class Deck
{
    private readonly List<Card> _cards = [];
    private readonly Random _random = new();
    
    public int NumOfCards => _cards.Count;
    public IReadOnlyList<Card> Cards => _cards.AsReadOnly();
    public string[] CardValues { get; }
    
    public Deck(int numOfCards)
    {
        if (numOfCards <= 0)
            throw new ArgumentException("Deck must contain at least one card.", nameof(numOfCards));

        CardValues = CardsDeckMapper.MapCardsValueByNumOfCards(numOfCards);
        GenerateCards(numOfCards);
    }
    
    private void GenerateCards(int numOfCards)
    {
        var count = 0;
        foreach (var suit in CardConstants.Suits)
        {
            foreach (var value in CardValues)
            {
                if (count >= numOfCards) return;
                _cards.Add(new Card { Id = count++, Suit = suit, Value = value });
            }
        }
    }
    
    public void ShuffleCardsDeck()
    {
        var rnd = new Random();
        for (var i = 0; i < NumOfCards; i++)
        {
            var j = rnd.Next(0, NumOfCards - 1);
            (_cards[i], _cards[j]) = (Cards[j], Cards[i]);
        }
    }

    public void GiveCardsToPlayers(List<Player> players)
    {
        var numCardForOnePlayer = NumOfCards / players.Count;
        var c = 0;
        for (var i = 0; i < numCardForOnePlayer / 2; i++)
        {
            for (var j = 0; j < players.Count; j++)
            {
                players[j].GiveCard(Cards[c++]);
                players[j].GiveCard(Cards[c++]);
            }
        }
    }
}