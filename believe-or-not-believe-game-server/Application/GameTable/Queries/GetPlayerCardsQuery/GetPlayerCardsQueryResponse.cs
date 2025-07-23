using Domain.Dtos;

namespace Application.GameTable.Queries.GetPlayerCardsQuery
{
    public class GetPlayerCardsQueryResponse
    {
        public bool Success { get; set; } = false;
        public string Message { get; set; } = string.Empty;
        public List<PlayerCardsDto> PlayersCards { get; set; } = new List<PlayerCardsDto>();
    }
}
