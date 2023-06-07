using Microsoft.Extensions.Configuration;
using StarCorpBot;
using StarCorpBot.Resources;
using StarCorpBot.Services;
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
            var telegramServices = new TelegramServices(config, botClient, cancellationToken);
            var redmineServices = new RedmineServices(config);
            switch (update.Type)
            {
                case UpdateType.Message:
                    if (update.Message is not { } message)
                        return;

                    if (message.Text is not { } messageText)
                        return;

                    var responseMessage = new RedmineTaskModel();

                    string lowerText = message.Text.ToLower();

                    if (lowerText.StartsWith($"{Commands.Start}"))
                    {
                        await telegramServices.SendMessage("Escolha uma opção:", KeyboardServices.CreateGetTasksKeyboard());
                    }
                    else if (lowerText.StartsWith($"{Commands.Today_Tasks}"))
                    {
                        var date = DateTime.Now;
                        responseMessage = await redmineServices.GetDailyTasks(date);
                        await telegramServices.SendDailyTasks(responseMessage, date, redmineServices);
                    }
                    else if (lowerText.StartsWith($"{Commands.Yesterday_Tasks}"))
                    {
                        var date = DateTime.Now.AddDays(-1);
                        responseMessage = await redmineServices.GetDailyTasks(date);
                        await telegramServices.SendDailyTasks(responseMessage, date, redmineServices);
                    }
                    else if (lowerText.StartsWith($"{Commands.Task_On_Date}"))
                    {
                        await telegramServices.SendMessage("Qual o dia desejado?", KeyboardServices.CreateDayKeyboard());
                    }
                    else if (lowerText.StartsWith($"{Commands.Get_Tasks_By_User}"))
                    {
                        var username = lowerText.Split(" ");

                        if (username?.Length < 2)
                        {
                            await telegramServices.SendMessage("Usuario não encontrado!");
                        }
                        else
                        {
                            var date = DateTime.Now;
                            responseMessage = await redmineServices.GetDailyTasks(date, username[1]);
                            await telegramServices.SendDailyTasks(responseMessage, date, redmineServices, username[1]);
                        }
                    }
                    else
                    {
                        await telegramServices.SendMessage("Não entendi ?!");
                    }
                    break;
                case UpdateType.CallbackQuery:
                    if (update.CallbackQuery.Data.StartsWith($"command_{CommandsMenu.Today_Tasks}"))
                    {
                        var date = DateTime.Now;
                        responseMessage = await redmineServices.GetDailyTasks(date);
                        await telegramServices.SendDailyTasks(responseMessage, date, redmineServices);
                    }
                    else if (update.CallbackQuery.Data.StartsWith($"command_{CommandsMenu.Yesterday_Tasks}"))
                    {
                        var date = DateTime.Now.AddDays(-1);
                        responseMessage = await redmineServices.GetDailyTasks(date);
                        await telegramServices.SendDailyTasks(responseMessage, date, redmineServices);
                    }
                    else if (update.CallbackQuery.Data.StartsWith($"command_{CommandsMenu.Task_On_Date}"))
                    {
                        await telegramServices.SendMessage("Qual o dia desejado?", KeyboardServices.CreateDayKeyboard());
                    }
                    else if (update.CallbackQuery.Data.StartsWith("day_"))
                    {
                        var day = update.CallbackQuery.Data.Replace("day_", "");

                        await telegramServices.SendMessage("Qual o mes desejado?", KeyboardServices.CreateMonthKeyboard(day.PadLeft(2, char.Parse("0"))));
                    }
                    else if (update.CallbackQuery.Data.StartsWith("month_"))
                    {
                        var month = update.CallbackQuery.Data.Split("_");
                        await telegramServices.SendMessage("Qual o ano desejado?", KeyboardServices.CreateYearKeyboard($"{month[1].PadLeft(2, char.Parse("0"))}-{month[2]}"));
                    }
                    else if (update.CallbackQuery.Data.StartsWith("year_"))
                    {
                        var year = update.CallbackQuery.Data.Split("_");

                        string format = "yyyy-MM-dd";

                        DateTime date;
                        bool isValid = DateTime.TryParseExact($"{year[1]}-{year[2]}", format, null, System.Globalization.DateTimeStyles.None, out date);

                        if (isValid)
                        {
                            responseMessage = await redmineServices.GetDailyTasks(date);
                            await telegramServices.SendDailyTasks(responseMessage, date, redmineServices);
                        }
                        else
                        {
                            await telegramServices.SendMessage("Data inválida!");
                        }
                    }
                    
                    break;
                default:
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