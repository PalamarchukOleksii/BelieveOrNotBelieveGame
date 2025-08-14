using GameCore.Abstractions;
using GameCore.Services;
using Microsoft.Extensions.DependencyInjection;

namespace GameCore.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGameCore(this IServiceCollection services)
    {
        services.AddTransient<IBotMakeMoveService, BotMakeMoveService>();
        services.AddTransient<IBotFirstMoveService, BotFirstMoveService>();
        services.AddTransient<IBotNotFirstMoveService, BotNotFirstMoveService>();
        //services.AddScoped<IBotService, BotService>();

        services.AddSingleton<IGameSessionService, GameSessionService>();

        return services;
    }
}