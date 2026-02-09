namespace GameCore.Models;

public class Move
{
    public string CardValue { get; }
    public IReadOnlyList<int> CardsId { get; }
    
    public Move(string cardValue, IEnumerable<int> cardsId)
    {
        if (string.IsNullOrWhiteSpace(cardValue))
        {
            throw new ArgumentException("Card value cannot be null or empty.", nameof(cardValue));
        }

        var cards = cardsId.ToList();

        if (cards.Count == 0)
        {
            throw new ArgumentException("Move must contain at least one card ID.", nameof(cardsId));
        }

        CardValue = cardValue;
        CardsId = cards.AsReadOnly();
    }
}