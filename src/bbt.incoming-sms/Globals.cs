public static class Globals
{
    public static string[] defaultKeywords = { "MIGROS", "KREDI" };
    public static string[] samplingKeywords = { "MIGROS", "KREDI", "PARA", "BORC" };

    public const string StateStore = "statestore";
    public const string LastPooledAt = "last_pooled_at";
    public const string AllowedKeywords = "allowed_keywords";

    public const string Queue = "pubsub";
    public const string Process_Sms_Queue = "queue_process_sms";
    public const string Sampling_Queue = "queue_sampling";
    public const string Error_Queue = "queue_error";
}