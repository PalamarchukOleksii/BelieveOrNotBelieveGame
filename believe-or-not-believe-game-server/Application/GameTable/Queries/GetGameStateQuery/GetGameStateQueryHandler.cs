using Domain.Abstractions.GameAbstractions;
using Domain.Dtos;
using MediatR;

namespace Application.GameTable.Queries.GetGameStateQuery
{
    public class GetGameStateQueryHandler : IRequestHandler<GetGameStateQueryRequest, GetGameStateQueryResponse>
    {
        private readonly IGameTableService _gameTableService;

        public GetGameStateQueryHandler(IGameTableService gameTableService)
        {
            _gameTableService = gameTableService;
        }

        public Task<GetGameStateQueryResponse> Handle(GetGameStateQueryRequest request, CancellationToken cancellationToken)
        {
            Domain.Models.GameModels.GameTable? table = _gameTableService.GetGameTableByName(request.GameName);
            if (table is null)
            {
                return Task.FromResult(new GetGameStateQueryResponse
                {
                    Success = false,
                    Message = $"Game with name {request.GameName} do not exist"
                });
            }

            GameStateDto state = table.GetGameState();

            return Task.FromResult(new GetGameStateQueryResponse
            {
                Success = true,
                Message = $"Game state of game {request.GameName} resived",
                CurrentMovePlayerName = state.CurrentMovePlayerName,
                CurrentMovePlayerConnectionId = state.CurrentMovePlayerConnectionId,
                CurrentPlayerCanMakeMove = state.CurrentPlayerCanMakeMove,
                CurrentPlayerCanMakeAssume = state.CurrentPlayerCanMakeAssume,
                MakeMoveValue = state.MakeMoveValue,
                CardsOnTableCount = state.CardsOnTableCount,
            });
        }
    }
}
