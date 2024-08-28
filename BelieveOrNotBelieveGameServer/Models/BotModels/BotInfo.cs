namespace BelieveOrNotBelieveGameServer.Models.BotModels;

public class BotInfo
{
    public BotInfo(
        Player bot, 
        List<Player> otherPlayers, 
        List<PlayingCard> cardForDiscard, 
        Move lastMove, 
        string previousPlayerName, 
        string nextPlayerName)
    {
        Bot = bot;
        OtherPlayers = otherPlayers;
        AllPlayersCards = otherPlayers.SelectMany(x => x.PlayersCards).ToList();
        CardForDiscard = cardForDiscard;
        LastMove = lastMove;
        PreviousPlayerName = previousPlayerName;
        NextPlayerName = nextPlayerName;
    }

    public Player Bot { get; set; }

    public List<Player> OtherPlayers { get; set; }

    public List<PlayingCard> AllPlayersCards { get; set; }

    public List<PlayingCard> CardForDiscard { get; set; }

    public Move LastMove { get; set; }

    public string PreviousPlayerName { get; set; }

    public string NextPlayerName { get; set; }
}
