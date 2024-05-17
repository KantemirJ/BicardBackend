using Telegram.Bot;

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
}
