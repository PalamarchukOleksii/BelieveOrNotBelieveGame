using Domain.Abstractions.GameAbstractions;
using Domain.Models.GameModels;
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
            Domain.Models.GameModels.GameTable? table = _gameTableService.GetGameTableByName(request.GameName);
            if (table is null)
            {
                return Task.FromResult(new MakeMoveCommandResponse
                {
                    Success = false,
                    Message = $"Game with name {request.GameName} do not exist"
                });
            }

            Player? player = table.GetPlayerByName(request.CallerConnectionId);
            if (player is null)
            {
                return Task.FromResult(new MakeMoveCommandResponse
                {
                    Success = false,
                    Message = $"Player with connection id {request.CallerConnectionId} do not exist in game {request.GameName}"
                });
            }

            switch (table.CheckIfPlayerCanMakeMove(player, request.CardsId))
            {
                case "Can make move":
                    Move playerMove = new Move(player.Name, request.CardsValue, request.CardsId);
                    table.MakeMoveOnGameTable(playerMove);

                    return Task.FromResult(new MakeMoveCommandResponse
                    {
                        Success = true,
                        Message = $"{playerMove.PlayerName} threw {playerMove.CardsId.Count} {playerMove.CardValue}(s)"
                    });

                case "Zero cards":
                    return Task.FromResult(new MakeMoveCommandResponse
                    {
                        Success = false,
                        Message = "You can only make an assumption"
                    });

                case "Not this player's turn":
                    return Task.FromResult(new MakeMoveCommandResponse
                    {
                        Success = false,
                        Message = "It is not your turn"
                    });

                case "Do not have these cards":
                    return Task.FromResult(new MakeMoveCommandResponse
                    {
                        Success = false,
                        Message = "You don't have these cards"
                    });

                default:
                    return Task.FromResult(new MakeMoveCommandResponse
                    {
                        Success = false,
                        Message = "Invalid move"
                    });
            }
        }
    }
}
