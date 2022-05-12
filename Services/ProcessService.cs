using System.Text.Json;

public class ProcessService : IProcessService
{

    const string storeName = "statestore";
    const string key_allowlist = "allowed_prefix_list";

    readonly string[] defaults = { "MIGORS", "DRD", "KREDI" };

    private readonly ILogger<ProcessController> _logger;
    private readonly DaprClient _daprClient;

    public ProcessService(ILogger<ProcessController> logger, DaprClient daprClient)
    {
        _logger = logger;
        _daprClient = daprClient;
    }

    public async Task<SMS> Process(SMS message)
    {

        _logger.LogInformation("Processing Incoming - Message Id -{0}-  and Wire Id -{1}-", message.Id, message.WireId);
        message.UpdatedMessage = UnifyMessage(message.IncomingMessage);

        _logger.LogInformation("Processing Incoming - Message is updated from -{0}- to -{1}-", message.IncomingMessage, message.UpdatedMessage);

        message.Keyword = message.UpdatedMessage.Split(" ")[0];

        message.IsAllowed = await IsAllowedPrefix(message.Keyword);

        _logger.LogInformation("Processing Incoming -{0}", JsonSerializer.Serialize(message));

        return message;
    }



    public string UnifyMessage(string message)
    {
        return message.ToUpperInvariant()
            .Replace("Ğ", "G")
            .Replace("Ş", "S")
            .Replace("Ü", "U")
            .Replace("Ö", "O")
            .Replace("İ", "I")
            .Replace("Ç", "C");
    }

    public async ValueTask<bool> IsAllowedPrefix(string keyword)
    {
        var keywords = await getAllowedPrefixList();
        return keywords.Any(s => keyword.Contains(s));
    }

    private async ValueTask<string[]> getAllowedPrefixList()
    {
        List<string> allowedPrefixList = await _daprClient.GetStateAsync<List<string>>(storeName, key_allowlist);
        if (allowedPrefixList is null)
        {
            allowedPrefixList = new List<string>(defaults);
            await _daprClient.SaveStateAsync(storeName, key_allowlist, allowedPrefixList);
            _logger.LogInformation("Allowed prefix not found, creating...");
        }

        _logger.LogInformation("Allowed prefix list: {0}", allowedPrefixList);
        return allowedPrefixList.ToArray();
    }

}
