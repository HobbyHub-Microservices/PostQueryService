namespace PostQueryService.EventProcessor;

public interface IEventProcessor
{
    void ProcessEvent(string message);
}
