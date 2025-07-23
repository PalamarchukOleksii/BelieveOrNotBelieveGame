namespace Application.GameTable.Commands.MakeAssumeCommand
{
    public class MakeAssumeCommandResponse
    {
        public bool Success { get; set; } = false;
        public string Message { get; set; } = string.Empty;
        public bool EndGame { get; set; } = false;
    }
}
