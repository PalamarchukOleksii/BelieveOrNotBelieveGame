using GameCore.Enums;

namespace GameCore.Models.GameModels;

public class Player
{
    public Player()
    {
    }

    public Player(string playerConnectionId, string name)
    {
        PlayerConnectionId = playerConnectionId;
        Name = name;
    }

    public Player(string playerConnectionId, string name, bool startGame, BotDificulty botDifficulty, bool isBot,
        List<Card> playersCards)
    {
        PlayerConnectionId = playerConnectionId;
        Name = name;
        StartGame = startGame;
        BotDifficulty = botDifficulty;
        IsBot = isBot;
        PlayersCards = playersCards;
    }

    public string PlayerConnectionId { get; private set; } = string.Empty;

    public string Name { get; private set; } = string.Empty;

    public List<Card> PlayersCards { get; private set; } = new();

    public bool StartGame { get; private set; }

    public BotDificulty BotDifficulty { get; private set; } = BotDificulty.ItIsNotABot;

    public bool IsBot { get; private set; }

    public void OnGameEnd()
    {
        StartGame = false;
        PlayersCards = new List<Card>();
    }

    public bool CheckIfPlayerHaveSomeCards(IReadOnlyList<int> cardsId)
    {
        var count = 0;
        foreach (var id in cardsId)
        foreach (var card in PlayersCards)
            if (Convert.ToInt32(id) == card.Id)
                count++;

        if (count == cardsId.Count) return true;

        return false;
    }

    public void SetStartGame(bool startGame)
    {
        StartGame = startGame;
    }
}