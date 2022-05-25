using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Dapr;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDaprClient(builder =>
        builder.UseJsonSerializationOptions(
            new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true,
            }));

var app = builder.Build();
app.UseCloudEvents();
app.UseRouting();
app.UseEndpoints(endpoints => endpoints.MapSubscribeHandler());

app.MapPost("/migros", async ([FromBody] Message message) =>
{
    app.Logger.LogInformation("Migros application is hit. Id : {0}", message.Id);
    return Results.Accepted("/", message.Id);
}).WithTopic("pubsub", "MIGROS");

app.Run();


public class Message
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string WireId { get; set; } = string.Empty;
    public string IncomingMessage { get; set; } = string.Empty;
    public string UpdatedMessage { get; set; } = string.Empty;
    public string Keyword { get; set; } = string.Empty;
    public bool IsAllowed { get; set; } = false;
    
}
