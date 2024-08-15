namespace BelieveOrNotBelieveGameServer.Models
{
    public class PlayingCard
    {
        public int Id { get; set; }
        public string CardSuit { get; set; } = string.Empty;
        public string CardValue { get; set; } = string.Empty;
    }
}
