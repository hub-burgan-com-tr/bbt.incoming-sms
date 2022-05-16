[ApiController]
[Route("[controller]")]
public class SamplingController : ControllerBase
{

    private readonly ILogger<SamplingController> _logger;

    public SamplingController(ILogger<SamplingController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    public async void GenerateSample([FromServices] DaprClient daprClient)
    {
        var lastPooled = await daprClient.GetStateAsync<DateTime>(Globals.StateStore, Globals.LastPooledAt);
        _logger.LogInformation("Pooling previous triger date: {0}", lastPooled);
        await Task.Delay(1000);
        lastPooled = DateTime.UtcNow;
        await daprClient.SaveStateAsync(Globals.StateStore, Globals.LastPooledAt, lastPooled);



        Random random = new Random();
        int samplesIndex = random.Next(Globals.samplingKeywords.Length);
        var sampleMessage = string.Format("{0} {1}666", Globals.samplingKeywords[samplesIndex], random.Next(10000000, 99999999).ToString());
        var message = new OperatorMessage { Message = sampleMessage, Id = Guid.NewGuid().ToString() };

        await daprClient.PublishEventAsync<OperatorMessage>(Globals.Queue, Globals.Sampling_Queue, message);
    }
}
