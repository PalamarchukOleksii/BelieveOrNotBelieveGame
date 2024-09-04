using Domain.Abstractions.GameAbstractions;
using Domain.Responses;
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
            GetPlayersCardsResponse result = _gameTableService.GetPlayersCards(request.GameName);

            return Task.FromResult(new GetPlayerCardsQueryResponse
            {
                Success = true,
                PlayersCards = result.PlayersCards
            });
        }
    }
}
