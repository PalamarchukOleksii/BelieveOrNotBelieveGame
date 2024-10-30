using Domain.Common.Options;
using MediatR;

namespace Application.GameTable.Commands.CreateGameCommand
{
    public class CreateGameCommandRequest : IRequest<CreateGameCommandResponse>
    {
        public GameTableOptions GameTableOptions { get; set; } = new GameTableOptions();
    }
}
