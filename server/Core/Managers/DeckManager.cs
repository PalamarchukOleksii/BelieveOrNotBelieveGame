using GameCore.Models;

namespace GameCore.Managers;

public class DeckManager
{
    public DeckManager(int numOfCards)
    {
        Deck = new Deck(numOfCards);
    }

    private Deck Deck { get; }
    public string[] DeckValues => Deck.CardValues;

    public void ShuffleDeck()
    {
        Deck.ShuffleCardsDeck();
    }

    public void DealCardsToPlayers(List<Player> players)
    {
        Deck.GiveCardsToPlayers(players);
    }
}