using Domain.Abstractions.GameAbstractions;
using Domain.Models.GameModels;
using MediatR;

namespace Application.GameTable.Commands.MakeAssumeCommand
{
    public class MakeAssumeCommandHandler : IRequestHandler<MakeAssumeCommandRequest, MakeAssumeCommandResponse>
    {
        private readonly IGameTableService _gameTableService;

        public MakeAssumeCommandHandler(IGameTableService gameTableService)
        {
            _gameTableService = gameTableService;
        }
        public Task<MakeAssumeCommandResponse> Handle(MakeAssumeCommandRequest request, CancellationToken cancellationToken)
        {
            Player? caller = _gameTableService.GetPlayerWithConnectionId(request.CallerConnectionId, request.GameName);

            if (caller == null)
            {
                throw new Exception("Player with this connection id does not exist");
            }

            _gameTableService.MakeAssume(request.GameName, request.IBelieve);

            throw new NotImplementedException();
        }
    }
}
