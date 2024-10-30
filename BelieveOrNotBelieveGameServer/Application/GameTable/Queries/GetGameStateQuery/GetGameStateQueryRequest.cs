using MediatR;

namespace Application.GameTable.Queries.GetGameStateQuery
{
    public class GetGameStateQueryRequest : IRequest<GetGameStateQueryResponse>
    {
        public string GameName { get; set; } = string.Empty;
    }
}
