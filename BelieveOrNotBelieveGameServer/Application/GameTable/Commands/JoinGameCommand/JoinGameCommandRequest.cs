using MediatR;

namespace Application.GameTable.Commands.JoinGameCommand
{
    public class JoinGameCommandRequest : IRequest<JoinGameCommandResponse>
    {
        public string Username { get; set; } = string.Empty;
        public string CallerConnectionId { get; set; } = string.Empty;
        public string GameName { get; set; } = string.Empty;
    }
}
