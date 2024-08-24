
using BelieveOrNotBelieveGameServer.Services.Abstraction;
using BelieveOrNotBelieveGameServer.Services.BotServices;

namespace BelieveOrNotBelieveGameServer.Configuration;

public class BotServiceConfiguration : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IBotFirstMoveService, BotFirstMoveService>();
        services.AddTransient<IBotNotFirstMoveService, BotNotFirstMoveService>();
        services.AddTransient<IBotService, BotService>();
    }
}
