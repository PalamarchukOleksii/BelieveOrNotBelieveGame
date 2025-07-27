using Presentation.Abstractions;
using Presentation.Constants;

namespace Presentation.Features.Heals;

public static class Check
{
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet($"{EndpointTags.Heals}/check", () => Results.Ok()).WithTags(EndpointTags.Heals);
        }
    }
}