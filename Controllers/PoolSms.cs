using Microsoft.AspNetCore.Mvc;

namespace bbt.incoming_sms.Controllers;

[ApiController]
[Route("[controller]")]
public class PoolSMSController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<PoolSMSController> _logger;

    public PoolSMSController(ILogger<PoolSMSController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    public void PoolSMS()
    {
        Console.Write("Triggered");
    }
}
