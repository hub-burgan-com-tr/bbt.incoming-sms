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
    public async Task<ActionResult> Process(SMS message, [FromServices] DaprClient daprClient, [FromServices] IPrefixService prefixService)
    {

        _logger.LogInformation("Processing Incoming - Message Id -{0}-  and Wire Id -{1}-", message.Id, message.WireId);

        message.UpdatedMessage = prefixService.UnifyMessage(message.IncomingMessage);
        _logger.LogInformation("Processing Incoming - Message is updated from -{0}- to -{1}-", message.IncomingMessage, message.UpdatedMessage);

        message.Keyword = message.UpdatedMessage.Split(" ")[0];

        var isAllowed = await prefixService.IsAllowedPrefix(message.Keyword);

        _logger.LogInformation("Processing Incoming - Root keyword is -{0}- and allowance is {1}", message.Keyword, isAllowed);


        await daprClient.PublishEventAsync<SMS>("pubsub", message.Keyword, message);

        return Ok();
    }

}
