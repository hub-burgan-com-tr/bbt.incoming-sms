
public class PrefixService : IPrefixService
{

    const string storeName = "statestore";
    const string key_allowlist = "allowed_prefix_list";

    readonly string[] defaults = { "MIGORS", "DRD", "KREDI" };

    private readonly ILogger<ProcessController> _logger;
    private readonly DaprClient _daprClient;

    public PrefixService(ILogger<ProcessController> logger, DaprClient daprClient)
    {
        _logger = logger;
        _daprClient = daprClient;
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
        return keyword.Contains(keyword.Trim());
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
