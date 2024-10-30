namespace Domain.Dtos
{
    public class GameStateDto
    {
        public string CurrentMovePlayerName { get; set; } = string.Empty;
        public string CurrentMovePlayerConnectionId { get; set; } = string.Empty;
        public bool CurrentPlayerCanMakeMove { get; set; } = false;
        public bool CurrentPlayerCanMakeAssume { get; set; } = false;
        public List<string> MakeMoveValue { get; set; } = new List<string>();
        public int CardsOnTableCount { get; set; } = 0;
    }
}
