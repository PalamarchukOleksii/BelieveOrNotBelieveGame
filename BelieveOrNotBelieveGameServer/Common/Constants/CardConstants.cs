namespace BelieveOrNotBelieveGameServer.Common.Constants;

public static class CardConstants
{
    public static readonly string[] Suits = ["Hearts", "Diamonds", "Clubs", "Spades"];

    public static readonly int CardNumber24 = 24;
    public static readonly int CardNumber36 = 36;
    public static readonly int CardNumber52 = 52;

    public static readonly string[] Values24 = ["9", "10", "Jack", "Queen", "King", "Ace"];
    public static readonly string[] Values36 = ["6", "7", "8", ..Values24];
    public static readonly string[] Values52 = ["2", "3", "4", "5", ..Values36];

    public static readonly int MaxCardsInOneMove = 4;
}
