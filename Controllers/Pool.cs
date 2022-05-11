[ApiController]
[Route("[controller]")]
public class PoolController : ControllerBase
{
    const string storeName = "statestore";
    const string key_lastpooled = "last_pooled_at";


    const string key_sms = "Process";

    private readonly ILogger<PoolController> _logger;

    public PoolController(ILogger<PoolController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    public async void Pool([FromServices] DaprClient daprClient)
    {
        var lastPooled = await daprClient.GetStateAsync<DateTime>(storeName, key_lastpooled);
        _logger.LogInformation("Pooling is trigered delta: {0}", lastPooled);

        await Task.Delay(1000);

        lastPooled = DateTime.UtcNow;
        await daprClient.SaveStateAsync(storeName, key_lastpooled, lastPooled);

        var message = new SMS { FullMessage = "KREDI 3656565255", WireId = Guid.NewGuid().ToString() };

        await daprClient.PublishEventAsync<SMS>("pubsub", key_sms, message);
    }
}
