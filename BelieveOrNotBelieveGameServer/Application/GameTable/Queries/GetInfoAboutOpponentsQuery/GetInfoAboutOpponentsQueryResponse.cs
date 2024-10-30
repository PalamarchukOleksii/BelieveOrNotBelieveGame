using Domain.Dtos;

namespace Application.GameTable.Queries.GetInfoAboutOpponentsQuery
{
    public class GetInfoAboutOpponentsQueryResponse
    {
        public bool Success { get; set; } = false;
        public string Message { get; set; } = string.Empty;
        public List<ShortOpponentInfoDto> OpponentInfo { get; set; } = new List<ShortOpponentInfoDto>();
        public List<string> PlayersConnectionIds { get; set; } = new List<string>();
        public List<string> PlayersWhoWinConnectionIds { get; set; } = new List<string>();
    }
}
