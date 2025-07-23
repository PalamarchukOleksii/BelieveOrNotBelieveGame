using MediatR;

namespace Application.GameTable.Queries.GetPlayerCardsQuery
{
    public class GetPlayerCardsQueryRequest : IRequest<GetPlayerCardsQueryResponse>
    {
        public string GameName { get; set; } = string.Empty;
    }
}
