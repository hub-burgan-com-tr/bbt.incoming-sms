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

    public async Task<Message> Process(Message message)
    {

        _logger.LogInformation("Processing Incoming - Message Id -{0}-  and Wire Id -{1}-", message.Id, message.WireId);
        message.UpdatedMessage = unifyMessage(message.IncomingMessage);

        _logger.LogInformation("Processing Incoming - Message is updated from -{0}- to -{1}-", message.IncomingMessage, message.UpdatedMessage);

        message.Keyword = message.UpdatedMessage.Split(" ")[0];

        message.IsAllowed = await isAllowedPrefix(message.Keyword);

        _logger.LogInformation("Processing Incoming -{0}", JsonSerializer.Serialize(message));

        return message;
    }



    private string unifyMessage(string message)
    {
        return message.ToUpperInvariant()
            .Replace("Ğ", "G")
            .Replace("Ş", "S")
            .Replace("Ü", "U")
            .Replace("Ö", "O")
            .Replace("İ", "I")
            .Replace("Ç", "C");
    }

    private async ValueTask<bool> isAllowedPrefix(string keyword)
    {
        var keywords = await getAllowedPrefixList();
        return keywords.Any(s => 
        {

             _logger.LogInformation("Result: {0} of {1} - {2}", s.Contains(keyword), s, keyword);
            
            return s.Contains(keyword);
            
        }
        );
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
