namespace Domain.Responses
{
    public class MakeMoveResponse
    {
        public bool Success { get; set; } = false;
        public string Message { get; set; } = string.Empty;
        public string CurrentMovePlayerName { get; set; } = string.Empty;
        public string CurrentMovePlayerConnectionId { get; set; } = string.Empty;
        public bool CurrentPlayerCanMakeMove { get; set; } = false;
        public int CardsOnTableCount { get; set; } = 0;
    }
}
