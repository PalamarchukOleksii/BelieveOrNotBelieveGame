namespace Application.GameTable.Commands.LeaveGameCommand
{
    public class LeaveGameCommandResponse
    {
        public bool Success { get; set; } = false;
        public string Message { get; set; } = string.Empty;
        public bool EndGame { get; set; } = false;
    }
}
