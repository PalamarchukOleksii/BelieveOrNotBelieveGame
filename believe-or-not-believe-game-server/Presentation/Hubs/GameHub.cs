using System.Collections.Concurrent;
using Domain.Abstractions.GameAbstractions;
using Microsoft.AspNetCore.SignalR;

namespace Presentation.Hubs;

public class GameHub(IGameTableService gameTableService) : Hub
{
    private static readonly ConcurrentDictionary<string, string> Connections = new();
    
    public static bool ConnectionExists(string connectionId)
    {
        return Connections.ContainsKey(connectionId);
    }

    public override async Task OnConnectedAsync()
    {
        Connections.TryAdd(Context.ConnectionId, Context.ConnectionId);
        await Clients.Caller.SendAsync("ReceiveConnectionId", Context.ConnectionId);

        await base.OnConnectedAsync();
    }
    
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var connectionId = Context.ConnectionId;
        try
        {
            var table = gameTableService.GetGameTableByConnectionId(connectionId);
            if (table is null)
                return;

            var player = table.GetPlayerByConnectionId(connectionId);
            if (player is null)
                return;

            bool result = gameTableService.PlayerLeaveGameTable(connectionId);
            if (!result)
                return;
            
            await Clients.Group(table.GameName)
                .SendAsync("RecievePlayerLeave", $"Player {player.Name} left the game");

            if (table.GameStarted)
            {
                table.EndGameTable();

                await Clients.Group(table.GameName)
                    .SendAsync("RecieveEndGame", $"Game with name {table.GameName} has ended");
            }
        }
        finally
        {
            Connections.TryRemove(connectionId, out _);
            await base.OnDisconnectedAsync(exception);
        }
    }
}