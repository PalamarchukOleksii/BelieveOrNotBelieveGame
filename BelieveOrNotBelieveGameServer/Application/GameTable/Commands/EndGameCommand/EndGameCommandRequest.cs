using MediatR;

namespace Application.GameTable.Commands.EndGameCommand
{
    public class EndGameCommandRequest : IRequest<EndGameCommandResponse>
    {
        public string GameName { get; set; } = string.Empty;
    }
}
