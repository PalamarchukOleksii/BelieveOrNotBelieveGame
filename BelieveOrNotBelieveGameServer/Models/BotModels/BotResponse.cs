namespace BelieveOrNotBelieveGameServer.Models.BotModels;

public class BotResponse
{
    public BotResponse(bool isNotBelieve, bool isBelieve, bool isMakeMove, Move? move)
    {
        IsNotBelieve = isNotBelieve;
        IsBelieve = isBelieve;
        IsMakeMove = isMakeMove;
        Move = move;
    }

    public bool IsNotBelieve { get; set; } = true;

    public bool IsBelieve { get; set; } = false;

    public bool IsMakeMove { get; set; } = false;

    public Move? Move { get; set; } = null;
}
