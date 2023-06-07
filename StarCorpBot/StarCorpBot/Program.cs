using Microsoft.Extensions.Configuration;
using StarCorpBot;
using StarCorpBot.Resources;
using StarCorpBot.Services;
using System.Resources;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using static System.Runtime.InteropServices.JavaScript.JSType;

internal class Program
{
    private static async Task Main(string[] args)
    {

        IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", false, true);
        IConfigurationRoot root = builder.Build();

        IConfiguration config = builder.Build();

        var botClient = new TelegramBotClient(config.GetRequiredSection("Api-Token").Value);

        using CancellationTokenSource cts = new();

        ReceiverOptions receiverOptions = new()
        {
            AllowedUpdates = Array.Empty<UpdateType>()
        };

        botClient.StartReceiving(
            updateHandler: HandleUpdateAsync,
            pollingErrorHandler: HandlePollingErrorAsync,
            receiverOptions: receiverOptions,
            cancellationToken: cts.Token
        );

        await botClient.SetMyCommandsAsync(CommandServices.LoadCommands());

        await botClient.GetMeAsync();

        Console.WriteLine("Bot Started!");
        Console.ReadLine();

        cts.Cancel();

        async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            Message message = update.Message ?? new Message();
            string messageText = (message.Text ?? string.Empty).ToLower();

            CallbackQuery callback = update.CallbackQuery ?? new CallbackQuery();
            string callbackText = (callback.Data ?? string.Empty).ToLower();

            var telegramServices = new TelegramServices(config, botClient, update, cancellationToken);
            var redmineServices = new RedmineServices(config);
            var responseMessage = new RedmineTaskModel();

            DateTime date = DateTime.Now;
            bool isValid;
            string format = "yyyy-MM-dd";
            string[] args = Array.Empty<string>();

            switch (true)
            {
                case true when messageText.StartsWith($"{Commands._01_Start}"):
                    await telegramServices.SendMessage("Escolha uma opção:", KeyboardServices.CreateGetTasksKeyboard());
                    break;
                case true when messageText.StartsWith($"{Commands._02_Help}"):
                case true when callbackText.StartsWith($"command_{Commands._02_Help}"):
                    var commands = CommandServices.LoadCommands();
                    var messageHelp =
@$"Comandos:
{Commands._01_Start} - {commands.FirstOrDefault(c => c.Command == Commands._01_Start)?.Description}
{Commands._02_Help} - {commands.FirstOrDefault(c => c.Command == Commands._02_Help)?.Description}
{Commands._03_Day_Tasks} - {commands.FirstOrDefault(c => c.Command == Commands._03_Day_Tasks)?.Description}                                 
{Commands._04_Previous_Day_Tasks} - {commands.FirstOrDefault(c => c.Command == Commands._04_Previous_Day_Tasks)?.Description}
{Commands._05_Tasks_By_Date} Optional: {{yyyy-MM-dd}} -  {commands.FirstOrDefault(c => c.Command == Commands._05_Tasks_By_Date)?.Description}
{Commands._06_User_Date_Tasks} {{username}} Optional: {{yyyy-MM-dd}} - {commands.FirstOrDefault(c => c.Command == Commands._06_User_Date_Tasks)?.Description}
";
                    await telegramServices.SendMessage(messageHelp.ToString());
                    break;
                case true when messageText.StartsWith($"{Commands._03_Day_Tasks}"):
                case true when callbackText.StartsWith($"command_{Commands._03_Day_Tasks}"):
                    responseMessage = await redmineServices.GetDailyTasks(date);
                    await telegramServices.SendDailyTasks(responseMessage, date, redmineServices);
                    break;
                case true when messageText.StartsWith($"{Commands._04_Previous_Day_Tasks}"):
                case true when callbackText.StartsWith($"command_{Commands._04_Previous_Day_Tasks}"):
                    date = DateTime.Now.AddDays(-1);
                    responseMessage = await redmineServices.GetDailyTasks(date);
                    await telegramServices.SendDailyTasks(responseMessage, date, redmineServices);
                    break;
                case true when messageText.StartsWith($"{Commands._05_Tasks_By_Date}"):
                case true when callbackText.StartsWith($"command_{Commands._05_Tasks_By_Date}"):
                    args = messageText.Split(' ');
                    if (args.Length > 1)
                    {
                        isValid = DateTime.TryParseExact($"{args[1]}", format, null, System.Globalization.DateTimeStyles.None, out date);
                        if (isValid)
                        {
                            responseMessage = await redmineServices.GetDailyTasks(date);
                            await telegramServices.SendDailyTasks(responseMessage, date, redmineServices);
                            break;
                        }
                        await telegramServices.SendMessage("Data inválida!");
                        break;
                    }
                    await telegramServices.SendMessage("Qual o dia desejado?", KeyboardServices.CreateDayKeyboard());
                    break;
                case true when messageText.StartsWith($"{Commands._06_User_Date_Tasks}"):
                case true when callbackText.StartsWith($"command_{Commands._06_User_Date_Tasks}"):
                    if (update.Type == UpdateType.CallbackQuery)
                    {
                        await telegramServices.SendMessage($"Uso do comando Inválido! O comando '{Commands._06_User_Date_Tasks}' é necessário parametros!");
                        break;
                    }
                    args = messageText.Split(' ');
                    if (args.Length > 1)
                    {
                        if (args.Length > 2)
                        {
                            isValid = DateTime.TryParseExact($"{args[2]}", format, null, System.Globalization.DateTimeStyles.None, out date);
                            if (isValid)
                            {
                                responseMessage = await redmineServices.GetDailyTasks(date, args[1]);
                                await telegramServices.SendDailyTasks(responseMessage, date, redmineServices, args[1]);
                                break;
                            }
                            await telegramServices.SendMessage("Data inválida!");
                            break;
                        }
                        responseMessage = await redmineServices.GetDailyTasks(date, args[1]);
                        await telegramServices.SendDailyTasks(responseMessage, date, redmineServices, args[1]);
                        break;
                    }
                    await telegramServices.SendMessage("Formato do comando inválido!");
                    break;
                case true when callbackText.StartsWith("day_"):
                    var day = callbackText.Replace("day_", "");
                    await telegramServices.SendMessage("Qual o mes desejado?", KeyboardServices.CreateMonthKeyboard(day.PadLeft(2, char.Parse("0"))));
                    break;
                case true when callbackText.StartsWith("month_"):
                    var month_day = callbackText.Split("_");
                    await telegramServices.SendMessage("Qual o ano desejado?", KeyboardServices.CreateYearKeyboard($"{month_day[1].PadLeft(2, char.Parse("0"))}-{month_day[2]}"));
                    break;
                case true when callbackText.StartsWith("year_"):
                    var year_month_day = callbackText.Split("_");
                    isValid = DateTime.TryParseExact($"{year_month_day[1]}-{year_month_day[2]}", format, null, System.Globalization.DateTimeStyles.None, out date);
                    if (isValid)
                    {
                        responseMessage = await redmineServices.GetDailyTasks(date);
                        await telegramServices.SendDailyTasks(responseMessage, date, redmineServices);
                        break;
                    }
                    await telegramServices.SendMessage("Data inválida!");
                    break;
                default:
                    await telegramServices.SendMessage("Não Entendi?!");
                    break;
            }
        }

        Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }
    }
}