using Presentation.Endpoints;

namespace Presentation.Features.Heals;

public static class CheckHeals
{
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("/heals", () => Results.Ok()).WithTags(EndpointTags.Heals);
        }
    }
}