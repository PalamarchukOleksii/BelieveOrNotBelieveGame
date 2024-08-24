using BelieveOrNotBelieveGameServer.Common.Constants;
using BelieveOrNotBelieveGameServer.Models.BotModels;

namespace BelieveOrNotBelieveGameServer.Common.Mappers;

public static class BotMapper
{
    public static double MapBotVisibilityByDificulty(BotDificulty botDificulty)
    {
        return botDificulty switch
        {
            BotDificulty.Easy => BotConstants.EasyDificultyVisibility,
            BotDificulty.Middle => BotConstants.MiddleDificultyVisibility,
            BotDificulty.Hard => BotConstants.HardDificultyVisibility,
            BotDificulty.VeryHard => BotConstants.VeryHardDificultyVisibility,
            _ => 0
        };
    }
}
