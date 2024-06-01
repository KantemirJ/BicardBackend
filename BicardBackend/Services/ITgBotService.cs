namespace BicardBackend.Services;

public interface ITgBotService
{
    public Task SendMessageAsync(string message);
    public Task SendPdfAsync(string filePath);
}
