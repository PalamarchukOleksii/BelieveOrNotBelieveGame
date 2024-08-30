namespace Application.GameTable.Commands.StartGameCommand
{
    public class StartGameCommandResponse
    {
        public string Result { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string CurrentMovePlayerName { get; set; } = string.Empty;
        public string CurrentMovePlayerConnectionId { get; set; } = string.Empty;
        public string[] MakeMoveValue { get; set; } = [];
    }
}
