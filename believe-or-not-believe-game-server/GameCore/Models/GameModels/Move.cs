namespace GameCore.Models.GameModels;

public class Move
{
    public Move(string playerName, string cardValue, IReadOnlyList<int> cardsId)
    {
        PlayerName = playerName;
        CardValue = cardValue;
        CardsId = cardsId;
    }

    public string PlayerName { get; private set; }
    public string CardValue { get; private set; }
    public IReadOnlyList<int> CardsId { get; private set; }
}