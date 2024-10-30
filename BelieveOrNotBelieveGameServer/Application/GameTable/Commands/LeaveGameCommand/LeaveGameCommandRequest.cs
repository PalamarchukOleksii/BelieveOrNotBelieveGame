using MediatR;

namespace Application.GameTable.Commands.LeaveGameCommand
{
    public class LeaveGameCommandRequest : IRequest<LeaveGameCommandResponse>
    {
        public string CallerConnectionId { get; set; } = string.Empty;
        public string GameName { get; set; } = string.Empty;
    }
}
