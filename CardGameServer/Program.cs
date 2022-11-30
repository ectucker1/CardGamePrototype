using System.Net;
using CardGameServer.Services.Lobby;
using CardGameServer.Websocket;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ILobbyService, LobbyService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddWebSocketManager();

var app = builder.Build();

var serviceScopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
var serviceProvider = serviceScopeFactory.CreateScope().ServiceProvider;

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseWebSockets();

app.MapGet("/lobby/create", () =>
{
    var lobbyService = serviceProvider.GetService<ILobbyService>();
    return lobbyService?.CreateLobby().Code;
});

app.MapWebSocketManager("/game", (HttpContext context) =>
{
    var components = context.Request.Path.ToString().Split("/");
    var lobby = components[^1];
    return serviceProvider.GetService<ILobbyService>()?.GetMessageHandler(lobby);
});

app.Run();
