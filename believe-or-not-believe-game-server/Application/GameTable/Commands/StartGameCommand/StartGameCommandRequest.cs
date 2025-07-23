using MediatR;

namespace Application.GameTable.Commands.StartGameCommand
{
    public class StartGameCommandRequest : IRequest<StartGameCommandResponse>
    {
        public string GameName { get; set; } = string.Empty;
        public string CallerConnectionId { get; set; } = string.Empty;
    }
}
