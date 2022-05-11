public class SMS
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string WireId { get; set; } = string.Empty;
    public string FullMessage { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}