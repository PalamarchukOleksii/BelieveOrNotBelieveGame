namespace Domain.Models.GameModels
{
    public class PlayingCard
    {
        public int Id { get; set; }
        public string Suit { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }
}
