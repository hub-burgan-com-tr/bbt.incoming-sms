using System.Text.Json;

public class ProcessService : IProcessService
{

    const string storeName = "statestore";
    const string key_allowlist = "allowed_prefix_list";

    readonly string[] defaults = { "MIGROS", "KREDI" };

    private readonly ILogger<ProcessController> _logger;
    private readonly DaprClient _daprClient;

    public ProcessService(ILogger<ProcessController> logger, DaprClient daprClient)
    {
        _logger = logger;
        _daprClient = daprClient;
    }

    public async Task<Message> Process(Message message)
    {
        _logger.LogInformation("Processing Incoming -{0}", JsonSerializer.Serialize(message));

        message.UpdatedMessage = unifyMessage(message.IncomingMessage);
        message.Keyword = message.UpdatedMessage.Split(" ")[0];

        message.IsAllowed = await isAllowedPrefix(message.Keyword);

        _logger.LogInformation("Processed Incoming -{0}", JsonSerializer.Serialize(message));

        if (message.IsAllowed)
        {
            await _daprClient.PublishEventAsync<Message>(Globals.Queue, message.Keyword, message);
        }
        else
        {
            await _daprClient.PublishEventAsync<Message>(Globals.Queue, Globals.Error_Queue, message);
        }

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
        return keywords.Any(s => s.Contains(keyword));
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

        return allowedPrefixList.ToArray();
    }

}
