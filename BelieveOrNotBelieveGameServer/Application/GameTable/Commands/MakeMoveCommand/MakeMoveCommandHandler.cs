using Domain.Abstractions.GameAbstractions;
using Domain.Models.GameModels;
using Domain.Responses;
using MediatR;

namespace Application.GameTable.Commands.MakeMoveCommand
{
    public class MakeMoveCommandHandler : IRequestHandler<MakeMoveCommandRequest, MakeMoveCommandResponse>
    {
        private readonly IGameTableService _gameTableService;

        public MakeMoveCommandHandler(IGameTableService gameTableService)
        {
            _gameTableService = gameTableService;
        }
        public Task<MakeMoveCommandResponse> Handle(MakeMoveCommandRequest request, CancellationToken cancellationToken)
        {
            Player? caller = _gameTableService.GetPlayerWithConnectionId(request.CallerConnectionId, request.GameName);

            if (caller == null)
            {
                throw new Exception("Player with this connection id does not exist");
            }

            MakeMoveResponse result = _gameTableService.MakeMove(request.GameName, caller, request.CardsValue, request.CardsId);

            return Task.FromResult(new MakeMoveCommandResponse
            {
                Success = result.Success,
                Message = result.Message,
                CurrentMovePlayerName = result.CurrentMovePlayerName,
                CurrentMovePlayerConnectionId = result.CurrentMovePlayerConnectionId,
                CurrentPlayerCanMakeMove = result.CurrentPlayerCanMakeMove,
                CardsOnTableCount = result.CardsOnTableCount
            });
        }
    }
}
