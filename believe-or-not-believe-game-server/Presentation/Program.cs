using Application;
using Domain;
using Hellang.Middleware.ProblemDetails;
using Presentation.Endpoints;
using Presentation.Hubs;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication()
    .AddDomain();

builder.Services.AddSignalR();

builder.Services.AddEndpoints();

builder.Services.AddOpenApi();

builder.Services.AddProblemDetails(configure =>
{
    configure.ValidationProblemStatusCode = StatusCodes.Status400BadRequest;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("ClientCors", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

//app.UseHttpsRedirection();
app.UseProblemDetails();

app.UseAuthorization();

app.MapEndpoints("api");

app.MapHub<GameHub>("game-hub");

app.UseCors("ClientCors");

await app.RunAsync();