using MediatR;

namespace Application.GameTable.Queries.GetInfoAboutOpponentsQuery
{
    public class GetInfoAboutOpponentsQueryRequest : IRequest<GetInfoAboutOpponentsQueryResponse>
    {
        public string GameName { get; set; } = string.Empty;
    }
}
