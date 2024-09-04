using Domain.Dtos;

namespace Domain.Responses
{
    public class GetInfoAboutOpponentsResponse
    {
        public bool Success { get; set; } = false;
        public List<OpponentInfoDto> OpponentInfo { get; set; } = new List<OpponentInfoDto>();
    }
}
