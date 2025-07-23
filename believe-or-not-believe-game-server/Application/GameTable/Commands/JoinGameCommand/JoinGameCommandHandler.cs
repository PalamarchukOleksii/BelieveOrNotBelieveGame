using Domain.Abstractions.GameAbstractions;
using Domain.Models.GameModels;
using MediatR;

namespace Application.GameTable.Commands.JoinGameCommand
{
    public class JoinGameCommandHandler : IRequestHandler<JoinGameCommandRequest, JoinGameCommandResponse>
    {
        private readonly IGameTableService _gameTableService;

        public JoinGameCommandHandler(IGameTableService gameTableService)
        {
            _gameTableService = gameTableService;
        }

        public Task<JoinGameCommandResponse> Handle(JoinGameCommandRequest request, CancellationToken cancellationToken)
        {
            Domain.Models.GameModels.GameTable? table = _gameTableService.GetGameTableByName(request.GameName);
            if (table is null)
            {
                return Task.FromResult(new JoinGameCommandResponse
                {
                    Success = false,
                    Message = $"Game with name {request.GameName} do not exist"
                });
            }

            Player? player = table.GetPlayerByName(request.Username);
            if (player is not null)
            {
                return Task.FromResult(new JoinGameCommandResponse
                {
                    Success = false,
                    Message = $"Player with username {request.Username} already exists in game {request.GameName}, please change the username"
                });
            }

            bool result = table.JoinGameTable(request.Username, request.CallerConnectionId);
            if (result)
            {
                return Task.FromResult(new JoinGameCommandResponse
                {
                    Success = true,
                    Message = $"Player {request.Username} joined game"
                });
            }

            return Task.FromResult(new JoinGameCommandResponse
            {
                Success = false,
                Message = "Error on server, player not join"
            });
        }
    }
}
