
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

    [Topic(Globals.Queue, Globals.Process_Sms_Queue)]
    [HttpPost]
    public async Task<ActionResult> Process(Message message, [FromServices] DaprClient daprClient, [FromServices] IProcessService processsor)
    {
        message = await processsor.Process(message);
        await daprClient.PublishEventAsync<Message>("pubsub", message.Keyword, message);
        return Ok();
    }
}
