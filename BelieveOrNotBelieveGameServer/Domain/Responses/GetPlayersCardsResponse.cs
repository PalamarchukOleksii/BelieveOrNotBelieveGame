using Domain.Dtos;

namespace Domain.Responses
{
    public class GetPlayersCardsResponse
    {
        public bool Success { get; set; } = false;
        public List<PlayerCardsDto> PlayersCards { get; set; } = new List<PlayerCardsDto>();
    }
}
