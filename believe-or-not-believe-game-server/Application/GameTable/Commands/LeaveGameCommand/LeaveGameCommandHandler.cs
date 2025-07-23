using Domain.Abstractions.GameAbstractions;
using Domain.Models.GameModels;
using MediatR;

namespace Application.GameTable.Commands.LeaveGameCommand
{
    public class LeaveGameCommandHandler : IRequestHandler<LeaveGameCommandRequest, LeaveGameCommandResponse>
    {
        private readonly IGameTableService _gameTableService;

        public LeaveGameCommandHandler(IGameTableService gameTableService)
        {
            _gameTableService = gameTableService;
        }

        public Task<LeaveGameCommandResponse> Handle(LeaveGameCommandRequest request, CancellationToken cancellationToken)
        {
            Domain.Models.GameModels.GameTable? table = _gameTableService.GetGameTableByName(request.GameName);
            if (table is null)
            {
                return Task.FromResult(new LeaveGameCommandResponse
                {
                    Success = false,
                    Message = $"Game with name {request.GameName} do not exist"
                });
            }

            Player? player = table.GetPlayerByConnectionId(request.CallerConnectionId);
            if (player is null)
            {
                return Task.FromResult(new LeaveGameCommandResponse
                {
                    Success = false,
                    Message = $"Player with connection id {request.CallerConnectionId} do not exist in game {request.GameName}"
                });
            }

            table.Players.Remove(player);

            return Task.FromResult(new LeaveGameCommandResponse
            {
                Success = true,
                Message = $"Player {player.Name} leave game",
                EndGame = table.GameStarted
            });
        }
    }
}
