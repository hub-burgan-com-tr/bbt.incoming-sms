public class SMS
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string WireId { get; set; } = string.Empty;
    public string IncomingMessage { get; set; } = string.Empty;
    public string UpdatedMessage { get; set; } = string.Empty;
    public string Keyword { get; set; } = string.Empty;
    public bool IsAllowed { get; set; } = false;
    
}