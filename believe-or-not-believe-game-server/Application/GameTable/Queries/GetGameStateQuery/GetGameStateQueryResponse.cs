namespace Application.GameTable.Queries.GetGameStateQuery
{
    public class GetGameStateQueryResponse
    {
        public bool Success { get; set; } = false;
        public string Message { get; set; } = string.Empty;
        public string CurrentMovePlayerName { get; set; } = string.Empty;
        public string CurrentMovePlayerConnectionId { get; set; } = string.Empty;
        public bool CurrentPlayerCanMakeMove { get; set; } = false;
        public bool CurrentPlayerCanMakeAssume { get; set; } = false;
        public List<string> MakeMoveValue { get; set; } = new List<string>();
        public int CardsOnTableCount { get; set; } = 0;
    }
}
