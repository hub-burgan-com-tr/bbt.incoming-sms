
public interface IProcessService
{
    Task<Message> Process(Message message);
}
