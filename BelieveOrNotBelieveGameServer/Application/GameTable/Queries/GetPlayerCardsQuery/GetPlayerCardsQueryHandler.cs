using Domain.Abstractions.GameAbstractions;
using Domain.Dtos;
using MediatR;

namespace Application.GameTable.Queries.GetPlayerCardsQuery
{
    internal class GetPlayerCardsQueryHandler : IRequestHandler<GetPlayerCardsQueryRequest, GetPlayerCardsQueryResponse>
    {
        private readonly IGameTableService _gameTableService;

        public GetPlayerCardsQueryHandler(IGameTableService gameTableService)
        {
            _gameTableService = gameTableService;
        }
        public Task<GetPlayerCardsQueryResponse> Handle(GetPlayerCardsQueryRequest request, CancellationToken cancellationToken)
        {
            Domain.Models.GameModels.GameTable? table = _gameTableService.GetGameTableByName(request.GameName);
            if (table is null)
            {
                return Task.FromResult(new GetPlayerCardsQueryResponse
                {
                    Success = false,
                    Message = $"Game with name {request.GameName} do not exist"
                });
            }

            List<PlayerCardsDto> cards = table.GetPlayerCards();

            return Task.FromResult(new GetPlayerCardsQueryResponse
            {
                Success = true,
                Message = $"Cards of players on game {request.GameName} resived",
                PlayersCards = cards
            });
        }
    }
}
