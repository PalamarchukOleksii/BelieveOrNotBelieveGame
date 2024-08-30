using Domain.Abstractions.BotAbstractions;
using Domain.Abstractions.GameAbstractions;
using Domain.Services.BotServices;
using Domain.Services.GameServices;
using Microsoft.Extensions.DependencyInjection;

namespace Domain
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDomain(this IServiceCollection services)
        {
            services.AddTransient<IBotMakeMoveService, BotMakeMoveService>();
            services.AddTransient<IBotFirstMoveService, BotFirstMoveService>();
            services.AddTransient<IBotNotFirstMoveService, BotNotFirstMoveService>();
            services.AddScoped<IBotService, BotService>();

            services.AddSingleton<IGameTableService, GameTableService>();

            return services;
        }
    }
}
