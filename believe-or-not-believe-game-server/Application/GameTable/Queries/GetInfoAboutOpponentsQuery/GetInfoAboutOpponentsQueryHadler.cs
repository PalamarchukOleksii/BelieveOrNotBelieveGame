using Domain.Abstractions.GameAbstractions;
using Domain.Dtos;
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
            Domain.Models.GameModels.GameTable? table = _gameTableService.GetGameTableByName(request.GameName);
            if (table is null)
            {
                return Task.FromResult(new GetInfoAboutOpponentsQueryResponse
                {
                    Success = false,
                    Message = $"Game with name {request.GameName} do not exist"
                });
            }

            List<ShortOpponentInfoDto> info = table.GetShortInfoAboutPlayers();
            (List<string> playersConnectionIds, List<string> playersWhoWinConnectionIds) = table.GetAllConnectionId();

            return Task.FromResult(new GetInfoAboutOpponentsQueryResponse
            {
                Success = true,
                Message = $"Short info about players on game {request.GameName} resived",
                OpponentInfo = info,
                PlayersConnectionIds = playersConnectionIds,
                PlayersWhoWinConnectionIds = playersWhoWinConnectionIds
            });
        }
    }
}
