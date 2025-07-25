using Domain.Models.GameModels;

namespace Presentation.Domain;

public class DeckManager
{
    public CardsDeck CardsDeck { get; }
    
    public DeckManager(int numOfCards)
    {
        CardsDeck = new CardsDeck(numOfCards);
    }
    
    public void InitializeDeck()
    {
        CardsDeck.GenerateCardsDeck();
        CardsDeck.ShuffleCardsDeck();
    }
    
    public void DealCardsToPlayers(List<Player> players)
    {
        CardsDeck.GiveCardsToPlayers(players);
    }
}