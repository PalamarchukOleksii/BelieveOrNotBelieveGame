namespace BelieveOrNotBelieveGameServer.Models
{
    public class Player
    {
        public string PlayerConnectionId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public List<PlayingCard> PlayersCards { get; set; } = new List<PlayingCard>();
        public bool StartGame { get; set; } = false;

        public bool CheckIfPlayerHaveSomeCards(string cardsId)
        {
            string[] ids = cardsId.Split(' ');
            int count = 0;

            foreach (string id in ids)
            {
                foreach (var card in PlayersCards)
                {
                    if (Convert.ToInt32(id) == card.Id)
                    {
                        count++;
                    }
                }
            }

            if(count == ids.Length)
            {
                return true;
            }

            return false;
        }
    }
}
