using BelieveOrNotBelieveGameServer.Models;

namespace BelieveOrNotBelieveGameServer.Services.Abstraction;

public interface IBotService
{
    void MakeMove(GameTable gameTable);
}
