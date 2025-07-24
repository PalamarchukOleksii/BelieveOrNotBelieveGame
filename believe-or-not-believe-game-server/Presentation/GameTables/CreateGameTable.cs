using Domain.Abstractions.GameAbstractions;
using Microsoft.AspNetCore.SignalR;
using Presentation.Endpoints;
using Presentation.Hubs;

namespace Presentation.GameTables;

public static class CreateGameTable
{
    public static async Task<IResult> HandleAsync(Request request, IHubContext<GameHub> hubContext,
        IGameTableService gameTableService)
    {
        var table = gameTableService.GetGameTableByName(request.GameName);

        if (table is not null) return Results.BadRequest($"Game with name {request.GameName} already exist");

        table = gameTableService.CreateGameTable(request.GameName, request.NumOfCards, request.MaxNumOfPlayers,
            request.AddBot);

        if (table is null) return Results.InternalServerError();

        await hubContext.Clients.All.SendAsync("RecieveNewGameCreated",
            new Response(table.GameName, table.NumOfCards, table.MaxNumOfPlayers, table.AddBot));

        //await JoinGameTable(createGameInfo.CreatorName, createGameInfo.GameName);

        return Results.Ok();
    }

    public record Request(
        string ConnectionId,
        string CreatorName,
        string GameName,
        int NumOfCards,
        int MaxNumOfPlayers,
        bool AddBot);

    public record Response(string GameName, int NumOfCards, int MaxNumOfPlayers, bool AddBot);

    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("game-table/create", HandleAsync);
        }
    }
}