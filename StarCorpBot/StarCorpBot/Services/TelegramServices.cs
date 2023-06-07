using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace StarCorpBot.Services
{
    public class TelegramServices
    {
        public static ITelegramBotClient botClient;
        public static CancellationToken cancellationToken;
        public static IConfiguration configuration;
        public TelegramServices(IConfiguration _configuration, ITelegramBotClient _botClient, CancellationToken _cancellationToken)
        {
            configuration = _configuration;
            botClient = _botClient;
            cancellationToken = _cancellationToken;
        }
        public async Task SendDailyTasks(RedmineTaskModel responseMessage, DateTime date, RedmineServices redmineServices, string? username = null)
        {
            await SendMessage("Busca Iniciada!");
            try
            {
                string linhaSeparadora = new string('-', 30);
                var users = new RedmineUserModel();

                if (string.IsNullOrEmpty(username))
                {
                    users = await redmineServices.GetUsers();
                }
                else
                {
                    users = await redmineServices.GetUser(username);
                }

                var issues = new RedmineIssueModel();

                if (responseMessage.TimeEntries?.Count > 0)
                {
                    issues = await redmineServices.GetByTasks(responseMessage.TimeEntries.Select(i => i.Issue.Id).ToList());
                }

                var usersTaskNotFound = new StringBuilder();
                usersTaskNotFound.AppendLine($"Data: <b>{date:dd/MM/yyyy}</b>\n");
                usersTaskNotFound.AppendLine($"Funcionários que não cadastraram as tarefas do dia:\n");

                foreach (var user in users.Users)
                {
                    if (user?.Id == 124)
                    {
                        continue;
                    }

                    var tasksByUsers = responseMessage.TimeEntries?.Where(p => p.User.Id == user?.Id).ToList();

                    if (tasksByUsers?.Count > 0)
                    {
                        var tasksByCompanies = tasksByUsers.GroupBy(c => c.Project.Id).ToList();

                        var messageBuilder = new StringBuilder();

                        messageBuilder.AppendLine($"Data: <b>{date:dd/MM/yyyy}</b>");
                        messageBuilder.AppendLine($"Funcionário: <b>{user.Firstname} {user.Lastname}</b>");

                        foreach (var tasksByCompany in tasksByCompanies)
                        {
                            messageBuilder.AppendLine($"\nProjeto: <b>{tasksByCompany.FirstOrDefault()?.Project.Name}</b>");
                            var tasks = tasksByCompany.GroupBy(t => t.Issue.Id).ToList();
                            foreach (var task in tasks)
                            {
                                var tempo = task.Sum(t => t.Hours);
                                var time = TimeSpan.FromHours(tempo);
                                var issue = issues.Issues.FirstOrDefault(i => i.Id == task.FirstOrDefault()?.Issue.Id);
                                messageBuilder.Append($"<b>#{task.FirstOrDefault()?.Issue.Id}</b> - {task.FirstOrDefault()?.Comments} <b><i>({time.ToString("hh\\:mm")}) ({issue?.Status.Name})</i></b> ");

                                if (issue?.DueDate > DateTime.MinValue)
                                {
                                    messageBuilder.Append($"<b><i>(Previsão: {issue.DueDate.GetValueOrDefault():dd/MM/yyyy})</i></b>");
                                }

                                if (tasks.Last() != task)
                                {
                                    messageBuilder.Append($"\n{linhaSeparadora}\n");
                                }
                            }

                            if (tasksByCompanies.Last() != tasksByCompany)
                            {
                                messageBuilder.Append($"\n");
                            }
                        }
                        await SendMessage(messageBuilder.ToString());
                    }
                    else
                    {
                        usersTaskNotFound.AppendLine($"Funcionário: <b>{user.Firstname} {user.Lastname}</b>");
                    }
                }

                await SendMessage(usersTaskNotFound.ToString());
            }
            catch (Exception e)
            {
                await SendMessage($"{e.Message} - Json: {JsonConvert.SerializeObject(e)}");
                throw;
            }
            finally
            {
                await SendMessage("Busca Finalizada!");
            }
        }
        public async Task SendMessage(string message, InlineKeyboardMarkup? inlineKeyboardMarkup = null)
        {
            await Task.Delay(TimeSpan.FromSeconds(2));

            var chatId = configuration.GetSection("Chat-Id").Value;
            Message sentMessage;

            if (inlineKeyboardMarkup is null)
            {
                sentMessage = await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    parseMode: ParseMode.Html,
                    text: message,
                    cancellationToken: cancellationToken);

                return;
            }

            sentMessage = await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    parseMode: ParseMode.Html,
                    text: message,
                    replyMarkup: inlineKeyboardMarkup,
                    cancellationToken: cancellationToken);
        }
    }
}
