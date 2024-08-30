using Domain.Abstractions.GameAbstractions;
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
            string result = _gameTableService.JoinGame(request.GameName, request.Username, request.CallerConnectionId);

            if (result == "Player join game")
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
                Message = $"Player with username {request.Username} already exists, please change the username"
            });
        }
    }
}
