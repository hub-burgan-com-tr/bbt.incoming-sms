[ApiController]
[Route("[controller]")]
public class ProcessController : ControllerBase
{

    const string key_sms = "Process";

    private readonly ILogger<ProcessController> _logger;

    public ProcessController(ILogger<ProcessController> logger)
    {
        _logger = logger;
    }

    [Topic("pubsub", key_sms)]
    [HttpPost]
    public async Task<ActionResult> Process(SMS message, [FromServices] DaprClient daprClient)
    {
        var root_keyword = message.FullMessage.Split(" ")[0];
        _logger.LogInformation("Processing Incoming sms -{0}- with root keyword -{1}-", message.Id, root_keyword);
        _logger.LogInformation("Processing Incoming sms with wireid -{0}- and message -{1}-", message.WireId, message.FullMessage);


        await daprClient.PublishEventAsync<SMS>("pubsub", root_keyword, message);

        return Ok();
    }
}
