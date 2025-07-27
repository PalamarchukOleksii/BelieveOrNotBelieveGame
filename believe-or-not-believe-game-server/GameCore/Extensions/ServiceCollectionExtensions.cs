using GameCore.Abstractions.BotAbstractions;
using GameCore.Abstractions.GameAbstractions;
using GameCore.Services.BotServices;
using GameCore.Services.GameServices;
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

        services.AddSingleton<IGameTableService, GameTableService>();

        return services;
    }
}