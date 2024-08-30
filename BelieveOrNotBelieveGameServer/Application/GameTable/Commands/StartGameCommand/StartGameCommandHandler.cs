using Domain.Abstractions.GameAbstractions;
using Domain.Dtos.Responses;
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

            Player? caller = _gameTableService.GetPlayerWithConnectionId(request.CallerConnectionId, request.GameName);

            if (caller == null)
            {
                throw new Exception("Player with this connection id does not exist");
            }

            StartGameResponse result = _gameTableService.StartGame(request.GameName, caller);

            return Task.FromResult(new StartGameCommandResponse
            {
                Result = result.Result,
                Message = result.Message,
                CurrentMovePlayerName = result.CurrentMovePlayerName,
                CurrentMovePlayerConnectionId = result.CurrentMovePlayerConnectionId,
                MakeMoveValue = result.MakeMoveValue
            });
        }
    }
}
