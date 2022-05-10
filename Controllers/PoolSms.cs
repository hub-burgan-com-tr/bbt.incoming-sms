[ApiController]
[Route("[controller]")]
public class PoolSMSController : ControllerBase
{
    const string storeName = "statestore";
    const string key = "last_pooled";

    private readonly ILogger<PoolSMSController> _logger;

    public PoolSMSController(ILogger<PoolSMSController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    public async void PoolSMS([FromServices] DaprClient daprClient)
    {
        List<string> allowedPrefixList = await daprClient.GetStateAsync<List<string>>(storeName, "allowed_prefix_list");
        if (allowedPrefixList is null)
        {
            allowedPrefixList = new List<string> { "MIGORS", "DRD", "KREDI" };
            await daprClient.SaveStateAsync(storeName,  "allowed_prefix_list", allowedPrefixList);
            _logger.LogInformation("Allowed prefix not found. Created");
        }

        _logger.LogInformation("Allowed prefix list: {0}", allowedPrefixList);

        var lastPooled = await daprClient.GetStateAsync<DateTime>(storeName, key);

        _logger.LogInformation("trigered for: {0}", lastPooled);

        await Task.Delay(1000);

        lastPooled = DateTime.UtcNow;
        await daprClient.SaveStateAsync(storeName, key, lastPooled);
    }
}
