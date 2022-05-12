
public interface IProcessService
{
    ValueTask<bool> IsAllowedPrefix(string keyword);
    string UnifyMessage(string message);

    Task<SMS> Process(SMS message);
}
