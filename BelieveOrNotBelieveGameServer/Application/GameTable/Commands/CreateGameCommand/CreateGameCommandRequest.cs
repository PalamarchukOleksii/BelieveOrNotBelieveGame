using MediatR;

namespace Application.GameTable.Commands.CreateGameCommand
{
    public class CreateGameCommandRequest : IRequest<CreateGameCommandResponse>
    {
        public string GameName { get; set; } = string.Empty;
        public int NumOfCards { get; set; } = 0;
        public int MaxNumOfPlayers { get; set; } = 0;
        public bool AddBot { get; set; } = false;
    }
}
