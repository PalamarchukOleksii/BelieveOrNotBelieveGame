using MediatR;

namespace Application.GameTable.Queries.GetGameNameByConnectionIdQuery
{
    public class GetGameNameByConnectionIdQueryRequest : IRequest<GetGameNameByConnectionIdQueryResponse>
    {
        public string CallerConnectionId { get; set; } = string.Empty;
    }
}
