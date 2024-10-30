namespace Domain.Models.GameModels
{
    public class Move
    {
        public string PlayerName { get; private set; }
        public string CardValue { get; private set; }
        public List<int> CardsId { get; private set; }

        public Move(string playerName, string cardValue, List<int> cardsId)
        {
            PlayerName = playerName;
            CardValue = cardValue;
            CardsId = cardsId;
        }
    }
}
