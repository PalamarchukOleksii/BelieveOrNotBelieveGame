﻿using MediatR;

namespace Application.GameTable.Commands.MakeMoveCommand
{
    public class MakeMoveCommandRequest : IRequest<MakeMoveCommandResponse>
    {
        public string GameName { get; set; } = string.Empty;
        public string CallerConnectionId { get; set; } = string.Empty;
        public string CardsValue { get; set; } = string.Empty;
        public string CardsId { get; set; } = string.Empty;
    }
}
