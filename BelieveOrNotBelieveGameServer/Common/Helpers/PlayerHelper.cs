using BelieveOrNotBelieveGameServer.Models;

namespace BelieveOrNotBelieveGameServer.Common.Helpers;

public static class PlayerHelper
{
    public static bool CheckIfPlayerHaveSomeCards(Player player,string cardsId)
    {
        string[] ids = cardsId.Split(' ');
        int count = 0;

        foreach (string id in ids)
        {
            foreach (var card in player.PlayersCards)
            {
                if (Convert.ToInt32(id) == card.Id)
                {
                    count++;
                }
            }
        }

        if(count == ids.Length)
        {
            return true;
        }

        return false;
    }
}
