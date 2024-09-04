using Domain.Abstractions.GameAbstractions;
using Domain.Responses;
using MediatR;

namespace Application.GameTable.Queries.GetInfoAboutOpponentsQuery
{
    public class GetInfoAboutOpponentsQueryHadler : IRequestHandler<GetInfoAboutOpponentsQueryRequest, GetInfoAboutOpponentsQueryResponse>
    {
        private readonly IGameTableService _gameTableService;

        public GetInfoAboutOpponentsQueryHadler(IGameTableService gameTableService)
        {
            _gameTableService = gameTableService;
        }
        public Task<GetInfoAboutOpponentsQueryResponse> Handle(GetInfoAboutOpponentsQueryRequest request, CancellationToken cancellationToken)
        {
            GetInfoAboutOpponentsResponse result = _gameTableService.GetInfoAboutOpponents(request.GameName);

            List<string> playersConnectionIds = _gameTableService.GetPlayersConnectionIds(request.GameName);
            List<string> playersWhoWinConnectionIds = _gameTableService.GetPlayersWhoWinConnectionIds(request.GameName);

            return Task.FromResult(new GetInfoAboutOpponentsQueryResponse
            {
                Success = result.Success,
                OpponentInfo = result.OpponentInfo,
                PlayersConnectionIds = playersConnectionIds,
                PlayersWhoWinConnectionIds = playersWhoWinConnectionIds
            });
        }
    }
}
