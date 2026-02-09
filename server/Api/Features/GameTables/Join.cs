using Microsoft.AspNetCore.SignalR;
using Presentation.Abstractions;
using Presentation.Constants;
using Presentation.Extensions;
using Presentation.Hubs;

namespace Presentation.Features.GameTables;

public static class Join
{
    public static async Task<IResult> HandleAsync(
        Request request,
        IHubContext<GameHub> hubContext,
        IGameTableService gameTableService)
    {
        if (!GameHub.ConnectionExists(request.ConnectionId)) return Results.BadRequest("Invalid connection ID");

        var table = gameTableService.GetGameTableByName(request.GameName);
        if (table is null) return Results.BadRequest($"GameModels with name {request.GameName} does not exist");

        var player = table.GetPlayerByName(request.Username);
        if (player is not null)
            return Results.BadRequest(
                $"Player with username {request.Username} already exists in game {request.GameName}");

        var result = gameTableService.PlayerJoinGameTable(request.Username, request.ConnectionId, request.GameName);
        if (!result) return Results.InternalServerError();

        await hubContext.Groups.AddToGroupAsync(request.ConnectionId, request.GameName);

        await hubContext.Clients.GroupExcept(request.GameName, request.ConnectionId)
            .SendAsync("ReceiveJoin", $"Player {request.Username} joined game");

        await hubContext.SendInfoAboutOpponentsAsync(table);

        return Results.Ok();
    }

    public record Request(string ConnectionId, string Username, string GameName);

    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost($"{EndpointTags.GameTables}/join", HandleAsync).WithTags(EndpointTags.GameTables);
        }
    }
}