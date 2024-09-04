namespace Domain.Responses
{
    public class StartGameResponse
    {
        public string Result { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string CurrentMovePlayerName { get; set; } = string.Empty;
        public string CurrentMovePlayerConnectionId { get; set; } = string.Empty;
        public string[] MakeMoveValue { get; set; } = [];
    }
}
