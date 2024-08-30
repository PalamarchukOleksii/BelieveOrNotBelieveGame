namespace Domain.Models.GameModels
{
    public class Move
    {
        public string PlayerName { get; set; }
        public string CardValue { get; set; }
        public List<int> CardsId { get; set; } = new List<int>();

        public Move(string playerName, string cardValue, string cardsId)
        {
            PlayerName = playerName;
            CardValue = cardValue;
            string[] ids = cardsId.Split(' ');
            foreach (var id in ids)
            {
                CardsId.Add(Convert.ToInt32(id));
            }
        }

        public Move(string playerName, string cardValue, List<int> cardsId)
        {
            PlayerName = playerName;
            CardValue = cardValue;
            CardsId = cardsId;
        }
    }
}
