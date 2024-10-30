using Domain.Abstractions.GameAbstractions;
using MediatR;

namespace Application.GameTable.Queries.GetGameNameByConnectionIdQuery
{
    public class GetGameNameByConnectionIdQueryHandler : IRequestHandler<GetGameNameByConnectionIdQueryRequest, GetGameNameByConnectionIdQueryResponse>
    {
        private readonly IGameTableService _gameTableService;

        public GetGameNameByConnectionIdQueryHandler(IGameTableService gameTableService)
        {
            _gameTableService = gameTableService;
        }

        public Task<GetGameNameByConnectionIdQueryResponse> Handle(GetGameNameByConnectionIdQueryRequest request, CancellationToken cancellationToken)
        {
            Domain.Models.GameModels.GameTable? table = _gameTableService.GetGameTableByConnectionId(request.CallerConnectionId);
            if (table is null)
            {
                return Task.FromResult(new GetGameNameByConnectionIdQueryResponse
                {
                    Success = false,
                    Message = $"Game with player with connection id {request.CallerConnectionId} do not exist"
                });
            }

            return Task.FromResult(new GetGameNameByConnectionIdQueryResponse
            {
                Success = true,
                GameName = table.Options.GameName
            });
        }
    }
}
