using Hellang.Middleware.ProblemDetails;
using ProblemDetailsOptions = Hellang.Middleware.ProblemDetails.ProblemDetailsOptions;

namespace Presentation.Configuration;

public class ApplicationServiceInstaller : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        //services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());

        AddFluentValidation<Program>(services);

        services.AddProblemDetails(ConfigureProblemDetailsOptions);
    }

    public void AddFluentValidation<TAssembly>(IServiceCollection services)
    {
        //    services.AddFluentValidationAutoValidation();
        //    services.AddFluentValidationClientsideAdapters();
        //    services.AddValidatorsFromAssemblyContaining<TAssembly>();
    }

    private static void ConfigureProblemDetailsOptions(ProblemDetailsOptions o)
    {
        o.ValidationProblemStatusCode = StatusCodes.Status400BadRequest;
    }
}