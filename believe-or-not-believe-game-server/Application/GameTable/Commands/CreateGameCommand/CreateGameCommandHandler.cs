using Domain.Abstractions.GameAbstractions;
using MediatR;

namespace Application.GameTable.Commands.CreateGameCommand
{
    public class CreateGameCommandHandler : IRequestHandler<CreateGameCommandRequest, CreateGameCommandResponse>
    {
        private readonly IGameTableService _gameTableService;

        public CreateGameCommandHandler(IGameTableService gameTableService)
        {
            _gameTableService = gameTableService;
        }

        public Task<CreateGameCommandResponse> Handle(CreateGameCommandRequest request, CancellationToken cancellationToken)
        {
            Domain.Models.GameModels.GameTable? table = _gameTableService.GetGameTableByName(request.GameTableOptions.GameName);

            if (table is not null)
            {
                return Task.FromResult(new CreateGameCommandResponse
                {
                    Success = false,
                    Message = $"Game with name {request.GameTableOptions.GameName} already exist"
                });
            }

            table = _gameTableService.CreateGameTable(request.GameTableOptions);

            if (table is null)
            {
                return Task.FromResult(new CreateGameCommandResponse
                {
                    Success = false,
                    Message = "Error on server, cant create game"
                });
            }

            return Task.FromResult(new CreateGameCommandResponse
            {
                Success = true,
                Message = $"Game {table.Options.GameName} created",
                GameTable = table
            });
        }
    }
}
