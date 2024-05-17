namespace BicardBackend.Services;

public interface ITgBotService
{
    public Task SendMessageAsync(string message);
}
