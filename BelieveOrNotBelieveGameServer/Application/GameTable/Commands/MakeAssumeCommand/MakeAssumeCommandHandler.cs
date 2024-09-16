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
            Domain.Models.GameModels.GameTable? table = _gameTableService.GetGameTableByName(request.GameName);
            if (table is null)
            {
                return Task.FromResult(new MakeAssumeCommandResponse
                {
                    Success = false,
                    Message = $"Game with name {request.GameName} do not exist"
                });
            }

            Player? player = table.GetPlayerByName(request.CallerConnectionId);
            if (player is null)
            {
                return Task.FromResult(new MakeAssumeCommandResponse
                {
                    Success = false,
                    Message = $"Player with connection id {request.CallerConnectionId} do not exist in game {request.GameName}"
                });
            }

            if (table.CheckIfPlayerCanMakeAssume(player))
            {
                (bool EndGame, string Message) = table.MakeAssumeOnGameTable(request.IBelieve);
                return Task.FromResult(new MakeAssumeCommandResponse
                {
                    Success = true,
                    Message = Message,
                    EndGame = EndGame
                });
            }

            return Task.FromResult(new MakeAssumeCommandResponse
            {
                Success = false,
                Message = $"You can not make assume",
            });
        }
    }
}
