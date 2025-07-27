using Hellang.Middleware.ProblemDetails;
using Presentation.Extensions;
using Presentation.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();

builder.Services.AddControllers();
builder.Services.AddEndpoints();

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

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => { options.CustomSchemaIds(type => type.FullName?.Replace("+", ".")); });

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.IdleTimeout = TimeSpan.FromHours(3);
});

builder.Services.AddHttpLogging(o => { });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseHttpLogging();

app.UseSession();

app.UseProblemDetails();

app.UseAuthorization();

app.MapEndpoints("api");

app.MapHub<GameHub>("game-hub");

app.UseCors("ClientCors");

await app.RunAsync();