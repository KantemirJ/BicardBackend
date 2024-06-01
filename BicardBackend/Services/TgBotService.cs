using Telegram.Bot;
using Telegram.Bot.Types;

namespace BicardBackend.Services;

public class TgBotService : ITgBotService
{
    private string token { get; } = "6861885303:AAGb06cdGC88emYO706xxamdze3dxAig-Lc";
    private long chatId = -4279778098;
    private TelegramBotClient client;

    public TgBotService()
    {
        client = new TelegramBotClient(token);
    }
    public async Task SendMessageAsync(string message)
    {
        await client.SendTextMessageAsync(chatId, message);
    }
    public async Task SendPdfAsync(string filePath)
    {
        if (!System.IO.File.Exists(filePath))
        {
            throw new ArgumentException($"PDF file not found: {filePath}");
        }

        using (var fileStream = System.IO.File.OpenRead(filePath))
        {
            await client.SendDocumentAsync(chatId, new InputFileStream(fileStream, Path.GetFileName(filePath)), cancellationToken: CancellationToken.None);
        }
    }


}
