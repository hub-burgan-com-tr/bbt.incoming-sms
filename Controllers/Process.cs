[ApiController]
[Route("[controller]")]
public class ProcessController : ControllerBase
{
    const string storeName = "statestore";
    
    const string key_sms = "Process";
    const string key_allowlist = "allowed_prefix_list";

    private readonly ILogger<ProcessController> _logger;

    public ProcessController(ILogger<ProcessController> logger)
    {
        _logger = logger;
    }

    [Topic("pubsub", key_sms)]
    [HttpPost]
    public async Task<ActionResult> Process(SMS message, [FromServices] DaprClient daprClient)
    {
         List<string> allowedPrefixList = await daprClient.GetStateAsync<List<string>>(storeName, key_allowlist);
        if (allowedPrefixList is null)
        {
            allowedPrefixList = new List<string> { "MIGORS", "DRD", "KREDI" };
            await daprClient.SaveStateAsync(storeName, key_allowlist, allowedPrefixList);
            _logger.LogInformation("Allowed prefix not found, creating..."); 
        }

        _logger.LogInformation("Allowed prefix list: {0}", allowedPrefixList);


        var root_keyword = message.FullMessage.Split(" ")[0];
        _logger.LogInformation("Processing Incoming sms -{0}- witßh root keyword -{1}-", message.Id, root_keyword);
        _logger.LogInformation("Processing Incoming sms with wireid -{0}- and message -{1}-", message.WireId, message.FullMessage);


        await daprClient.PublishEventAsync<SMS>("pubsub", root_keyword, message);

        return Ok();
    }



}
