
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
    public async Task<ActionResult> Process(SMS message, [FromServices] DaprClient daprClient, [FromServices] IProcessService processsor)
    {
        message = await processsor.Process(message);
        await daprClient.PublishEventAsync<SMS>("pubsub", message.Keyword, message);
        return Ok();
    }
}
