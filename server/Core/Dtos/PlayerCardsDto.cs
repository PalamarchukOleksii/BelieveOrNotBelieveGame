using GameCore.Models;

namespace GameCore.Dtos;

public class PlayerCardsDto
{
    public string PlayerConnectionId { get; set; } = string.Empty;
    public List<Card> Cards { get; set; } = new();
}