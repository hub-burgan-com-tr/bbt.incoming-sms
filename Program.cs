using System.Text.Json;
using Dapr;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IProcessService, ProcessService>();

builder.Services.AddControllers();
builder.Services.AddDaprClient(builder =>
        builder.UseJsonSerializationOptions(
            new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true,
            }));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();



app.MapControllers();

app.UseRouting();

app.UseAuthorization();

app.UseCloudEvents();

app.UseEndpoints(endpoints =>
{
    endpoints.MapSubscribeHandler(); 
    endpoints.MapControllers();
});

app.Run();
