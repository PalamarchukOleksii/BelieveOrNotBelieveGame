using Domain.Models.GameModels;

namespace Domain.Dtos
{
    public class PlayerCardsDto
    {
        public string PlayerConnectionId { get; set; } = string.Empty;
        public List<PlayingCard> Cards { get; set; } = new List<PlayingCard>();
    }
}
