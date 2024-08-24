namespace BelieveOrNotBelieveGameServer.Models.BotModels;

public class BotResponse
{
    public BotResponse(bool isBelieve, bool isNotBelieve, bool isMakeMove = false, Move? move = null)
    {
        IsNotBelieve = isNotBelieve;
        IsBelieve = isBelieve;
        IsMakeMove = isMakeMove;
        Move = move;
    }

    public BotResponse(Move move)
    {
        IsNotBelieve = false;
        IsBelieve = false;
        IsMakeMove = true;
        Move = move;
    }

    public BotResponse() {}

    public bool IsAssuming => IsNotBelieve == true || IsBelieve == true;

    public bool IsNotBelieve { get; set; } = true;

    public bool IsBelieve { get; set; } = false;

    public bool IsMakeMove { get; set; } = false;

    public Move? Move { get; set; } = null;
}
