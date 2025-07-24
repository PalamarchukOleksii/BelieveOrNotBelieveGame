using Presentation.Endpoints;

namespace Presentation.Heals;

public static class CheckHeals
{
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("/heals", () => Results.Ok());
        }
    }
}