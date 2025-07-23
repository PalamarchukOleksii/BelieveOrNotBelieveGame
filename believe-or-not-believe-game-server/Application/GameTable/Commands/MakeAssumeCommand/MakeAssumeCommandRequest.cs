using MediatR;

namespace Application.GameTable.Commands.MakeAssumeCommand
{
    public class MakeAssumeCommandRequest : IRequest<MakeAssumeCommandResponse>
    {
        public string GameName { get; set; } = string.Empty;
        public string CallerConnectionId { get; set; } = string.Empty;
        public bool IBelieve { get; set; } = false;
    }
}
