using Domain.Dtos;

namespace Application.GameTable.Queries.GetInfoAboutOpponentsQuery
{
    public class GetInfoAboutOpponentsQueryResponse
    {
        public bool Success { get; set; } = false;
        public List<OpponentInfoDto> OpponentInfo { get; set; } = new List<OpponentInfoDto>();
        public List<string> PlayersConnectionIds { get; set; } = new List<string>();
        public List<string> PlayersWhoWinConnectionIds { get; set; } = new List<string>();
    }
}
