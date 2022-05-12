[ApiController]
[Route("[controller]")]
public class SamplingController : ControllerBase
{
    const string storeName = "statestore";
    const string key_lastpooled = "last_pooled_at";


    const string key_sms = "Process";

    private readonly ILogger<SamplingController> _logger;

    public SamplingController(ILogger<SamplingController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    public async void GenerateSample([FromServices] DaprClient daprClient)
    {
        var lastPooled = await daprClient.GetStateAsync<DateTime>(storeName, key_lastpooled);
        _logger.LogInformation("Pooling is trigered delta: {0}", lastPooled);

        await Task.Delay(1000);

        lastPooled = DateTime.UtcNow;
        await daprClient.SaveStateAsync(storeName, key_lastpooled, lastPooled);

        var message = new Message { IncomingMessage = "KREDi 3656565255", WireId = Guid.NewGuid().ToString() };

        await daprClient.PublishEventAsync<Message>("pubsub", key_sms, message);
    }
}
