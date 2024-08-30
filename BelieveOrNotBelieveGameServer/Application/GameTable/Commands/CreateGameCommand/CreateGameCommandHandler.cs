using Domain.Abstractions.GameAbstractions;
using Domain.Common.Options;
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
            GameTableOptions options = new GameTableOptions
            {
                GameName = request.GameName,
                AddBot = request.AddBot,
                MaxNumOfPlayers = request.MaxNumOfPlayers,
                NumOfCards = request.NumOfCards
            };

            bool result = _gameTableService.CreateGame(options);

            if (!result)
            {
                return Task.FromResult(new CreateGameCommandResponse { Successs = false, Message = "Game with this name already exist" });
            }

            return Task.FromResult(new CreateGameCommandResponse { Successs = true, Message = $"Game {request.GameName} created" });
        }
    }
}
