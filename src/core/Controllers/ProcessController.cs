
[ApiController]
[Route("[controller]")]
public class ProcessController : ControllerBase
{
    private readonly ILogger<ProcessController> _logger;

    public ProcessController(ILogger<ProcessController> logger)
    {
        _logger = logger;
    }

    [Topic(Globals.Queue, Globals.Process_Sms_Queue)]
    [HttpPost]
    public async Task<ActionResult> Process(Message message, [FromServices] DaprClient daprClient, [FromServices] IProcessService processsor)
    {
        await processsor.Process(message);
        return Ok();
    }
}
