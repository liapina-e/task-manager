
using System.ComponentModel;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
namespace Library_dop;
/// <summary>
/// Класс, в котором реализова все действия тг бота.
/// </summary>
public class TelegramBot
{
    // Токен для создания бота.
    string token = "7334673343:AAFFbOgEmWa_4iMMYuSccBYg31rRrMOxFfE";
    private readonly ITelegramBotClient bot;    // Переменная самого бота.
    private Chat chat;  // Переменная хранит в себе чат, из которого пишет пользователь.
    
    /// <summary>
    /// Конструктор без параметров.
    /// </summary>
    public TelegramBot()
    {
        bot = new TelegramBotClient(token);
    }
    /// <summary>
    /// В методе запускаем бота.
    /// </summary>
    public async System.Threading.Tasks.Task StartAsync()
    {
        using CancellationTokenSource cts = new CancellationTokenSource();

        // Настройка обработки входящих сообщений. Написана с помощью GPT, т.к. данный материал не был изучен.
        ReceiverOptions receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = Array.Empty<UpdateType>() // Получаем все типы обновлений
        };

        bot.StartReceiving(updateHandler: HandleUpdateAsync, errorHandler: HandleErrorAsync, receiverOptions: receiverOptions, cancellationToken: cts.Token);

        User me = await bot.GetMe();
        
    }

    /// <summary>
    /// В методе обрабатываем входящие в бота сообщения.
    /// </summary>
    /// <param name="botClient"></param>
    /// <param name="update"></param>
    /// <param name="cancellationToken"></param>
    private async System.Threading.Tasks.Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        // Обрабатываем только текстовые сообщения.
        if (update.Type != UpdateType.Message || update.Message!.Type != MessageType.Text)
            return;

        string? messageText = update.Message.Text;
        chat = update.Message.Chat;

        if (messageText == "/start")
        {
            await botClient.SendMessage(chat.Id, "Привет! Я бот для напоминаний о дедлайнах. Ожидайте уведомлений!", cancellationToken: cancellationToken);
        }
    }

    /// <summary>
    /// В методе обрабатываем ошибки, чтоб бот не "падал".
    /// Написан с помощью GPT, т.к. материал является сложным и не изучен раннее.
    /// </summary>
    /// <param name="botClient"></param>
    /// <param name="exception"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private System.Threading.Tasks.Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        string errorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Ошибка Telegram API: {apiRequestException.ErrorCode}\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine(errorMessage);
        return System.Threading.Tasks.Task.CompletedTask;
    }

    /// <summary>
    /// В методе вызываем другой метод для отправки напоминания.
    /// Метод работает асинхронно.
    /// </summary>
    /// <param name="tasks"></param>
    public async System.Threading.Tasks.Task SendRemindersAsync(List<Task> tasks)
    {
        if (chat == null)
        {
            Console.WriteLine("Чат не установлен. Пользователь еще не отправил /start.");
            return;
        }

        Methods.RemindAboutDeadlineInTg(null, new DoWorkEventArgs(tasks), bot, chat);
    }
}