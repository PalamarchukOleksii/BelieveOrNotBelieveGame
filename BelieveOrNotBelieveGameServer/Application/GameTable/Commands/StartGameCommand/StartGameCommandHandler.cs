using Domain.Abstractions.GameAbstractions;
using Domain.Models.GameModels;
using MediatR;

namespace Application.GameTable.Commands.StartGameCommand
{
    public class StartGameCommandHandler : IRequestHandler<StartGameCommandRequest, StartGameCommandResponse>
    {
        private readonly IGameTableService _gameTableService;

        public StartGameCommandHandler(IGameTableService gameTableService)
        {
            _gameTableService = gameTableService;
        }

        public Task<StartGameCommandResponse> Handle(StartGameCommandRequest request, CancellationToken cancellationToken)
        {
            Domain.Models.GameModels.GameTable? table = _gameTableService.GetGameTableByName(request.GameName);
            if (table is null)
            {
                return Task.FromResult(new StartGameCommandResponse
                {
                    Success = false,
                    Message = $"Game with name {request.GameName} do not exist"
                });
            }

            Player? player = table.GetPlayerByConnectionId(request.CallerConnectionId);
            if (player is null)
            {
                return Task.FromResult(new StartGameCommandResponse
                {
                    Success = false,
                    Message = $"Player with connection id {request.CallerConnectionId} do not exist in game {request.GameName}"
                });
            }

            int countOfPlayersInOnGameTable = table.Players.Count;
            if (countOfPlayersInOnGameTable < 2)
            {
                return Task.FromResult(new StartGameCommandResponse
                {
                    Success = false,
                    Message = "Two players required to start the game"
                });
            }

            bool result = table.StartGameTable(player);
            if (result)
            {
                return Task.FromResult(new StartGameCommandResponse
                {
                    Success = result,
                    Message = $"Game started"
                });
            }

            return Task.FromResult(new StartGameCommandResponse
            {
                Success = result,
                Message = $"Waiting for other players to start the game"
            });
        }
    }
}
