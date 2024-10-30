using Domain.Abstractions.GameAbstractions;
using MediatR;

namespace Application.GameTable.Commands.EndGameCommand
{
    public class EndGameCommandHandler : IRequestHandler<EndGameCommandRequest, EndGameCommandResponse>
    {
        private readonly IGameTableService _gameTableService;

        public EndGameCommandHandler(IGameTableService gameTableService)
        {
            _gameTableService = gameTableService;
        }

        public Task<EndGameCommandResponse> Handle(EndGameCommandRequest request, CancellationToken cancellationToken)
        {
            Domain.Models.GameModels.GameTable? table = _gameTableService.GetGameTableByName(request.GameName);
            if (table is null)
            {
                return Task.FromResult(new EndGameCommandResponse
                {
                    Success = false,
                    Message = $"Game with name {request.GameName} do not exist"
                });
            }

            table.EndGameTable();
            return Task.FromResult(new EndGameCommandResponse
            {
                Success = true,
                Message = $"Game with name {request.GameName} end"
            });
        }
    }
}
