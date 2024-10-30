namespace Presentation.Dtos
{
    public class MakeMoveDto
    {
        public string GameName { get; set; } = string.Empty;
        public string CardsValue { get; set; } = string.Empty;
        public List<int> CardsId { get; set; } = new List<int>();
    }
}
