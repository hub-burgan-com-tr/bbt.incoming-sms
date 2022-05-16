[ApiController]
[Route("[controller]")]
public class SaveController : ControllerBase
{

    private readonly ILogger<SaveController> _logger;

    public SaveController(ILogger<SaveController> logger)
    {
        _logger = logger;
    }

    [Topic(Globals.Queue, Globals.Sampling_Queue)]
    [HttpPost]
    public async void Save(OperatorMessage operatorMessage, [FromServices] DaprClient daprClient)
    {
        var message = new Message
        {
            IncomingMessage = operatorMessage.Message,
            WireId = operatorMessage.Id
        };

        await daprClient.PublishEventAsync<Message>(Globals.Queue, Globals.Process_Sms_Queue, message);
    }
}
