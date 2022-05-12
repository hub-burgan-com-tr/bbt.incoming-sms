
public interface IPrefixService
{
    ValueTask<bool> IsAllowedPrefix(string keyword);
    string UnifyMessage(string message);
}
