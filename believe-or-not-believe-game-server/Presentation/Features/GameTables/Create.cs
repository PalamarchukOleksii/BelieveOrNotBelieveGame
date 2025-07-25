using Domain.Abstractions.GameAbstractions;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Presentation.Constants;
using Presentation.Endpoints;
using Presentation.Hubs;

namespace Presentation.Features.GameTables;

public static class Create
{
    public record Request(
        string ConnectionId,
        string CreatorName,
        string GameName,
        int NumOfCards,
        int MaxNumOfPlayers,
        bool AddBot);

    public record Response(string GameName, int NumOfCards, int MaxNumOfPlayers, bool AddBot);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost($"{EndpointTags.GameTables}/create", HandleAsync).WithTags(EndpointTags.GameTables);
        }
    }

    public static async Task<IResult> HandleAsync(
        Request request,
        IHubContext<GameHub> hubContext,
        IGameTableService gameTableService)
    {
        if (!GameHub.ConnectionExists(request.ConnectionId))
        {
            return Results.BadRequest("Invalid connection ID");
        }
        
        var table = gameTableService.GetGameTableByName(request.GameName);
        if (table is not null)
        {
            return Results.BadRequest($"Game with name {request.GameName} already exist");
        }

        table = gameTableService.CreateGameTable(
            request.GameName,
            request.NumOfCards,
            request.MaxNumOfPlayers,
            request.AddBot);

        if (table is null)
        {
            return Results.InternalServerError();
        }
        
        await hubContext.Clients.All.SendAsync("RecieveNewGameCreated",
            new Response(table.GameName, table.NumOfCards, table.MaxNumOfPlayers, table.AddBot));

        var result = gameTableService.PlayerJoinGameTable(request.CreatorName, request.ConnectionId, request.GameName);
        if (!result)
        {
            return Results.InternalServerError();
        }

        await hubContext.Groups.AddToGroupAsync(request.ConnectionId, request.GameName);

        return Results.Ok();
    }
}
