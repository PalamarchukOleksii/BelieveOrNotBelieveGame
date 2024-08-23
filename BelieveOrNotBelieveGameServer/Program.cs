using BelieveOrNotBelieveGameServer.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("ClientCors", policy =>
    {
        policy.WithOrigins("http://26.248.118.214:5173")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
});
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHub<GameHub>("game-hub");

app.UseCors("ClientCors");

app.Run("http://26.248.118.214:7075");
