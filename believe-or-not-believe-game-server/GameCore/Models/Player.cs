namespace GameCore.Models;

public class Player
{
    public string ConnectionId { get; private set; }
    public string Name { get; private set; }

    private readonly List<Card> _cards = [];
    public IReadOnlyList<Card> Cards => _cards.AsReadOnly();

    public bool IsReadyToStart { get; private set; }
    
    public Player(string connectionId, string name)
    {
        if (string.IsNullOrWhiteSpace(connectionId))
        {
            throw new ArgumentException("ConnectionId cannot be empty.", nameof(connectionId));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name cannot be empty.", nameof(name));
        }

        ConnectionId = connectionId;
        Name = name;
        IsReadyToStart = false;
    }
    
    public void StartGame()
    {
        if (IsReadyToStart)
        {
            throw new InvalidOperationException("Player is already marked as ready.");
        }

        _cards.Clear();
        IsReadyToStart = true;
    }

    public void EndGame()
    {
        _cards.Clear();
        IsReadyToStart = false;
    }

    public bool HasCards(IEnumerable<int> cardIds)
    {
        var requiredIds = new HashSet<int>(cardIds);
        var playerCardIds = _cards.Select(c => c.Id).ToHashSet();

        return requiredIds.All(id => playerCardIds.Contains(id));
    }
    
    public bool HasCard(int cardId)
    {
        return _cards.Any(c => c.Id == cardId);
    }
    
    public void GiveCard(Card card)
    {
        if (_cards.Any(c => c.Id == card.Id))
        {
            throw new InvalidOperationException("Player already has this card.");
        }

        _cards.Add(card);
    }
    
    public void GiveCards(IEnumerable<Card> cards)
    {
        if (cards == null) throw new ArgumentNullException(nameof(cards));

        foreach (var card in cards)
        {
            if (_cards.Any(c => c.Id == card.Id))
            {
                throw new InvalidOperationException($"Player already has card with Id {card.Id}.");
            }

            _cards.Add(card);
        }
    }

    public void RemoveCard(int cardId)
    {
        var cardToRemove = _cards.FirstOrDefault(c => c.Id == cardId);
        if (cardToRemove == null)
        {
            throw new InvalidOperationException("Player does not have this card.");
        }

        _cards.Remove(cardToRemove);
    }
}