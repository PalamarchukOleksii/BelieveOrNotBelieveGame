using GameCore.Models.GameModels;

namespace GameCore.Managers;

public class DeckManager
{
    public DeckManager(int numOfCards)
    {
        Deck = new Deck(numOfCards);
    }

    public Deck Deck { get; }

    public void InitializeDeck()
    {
        Deck.GenerateCardsDeck();
        Deck.ShuffleCardsDeck();
    }

    public void DealCardsToPlayers(List<Player> players)
    {
        Deck.GiveCardsToPlayers(players);
    }
}