namespace Presentation.Dtos;

public class CreateGameDto
{
    public string CreatorName { get; set; } = string.Empty;
    public string GameName { get; set; } = string.Empty;
    public int NumOfCards { get; set; } = 0;
    public int MaxNumOfPlayers { get; set; } = 0;
    public bool AddBot { get; set; } = false;
}