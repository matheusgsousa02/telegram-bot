using Telegram.Bot.Types.ReplyMarkups;

namespace StarCorpBot.Services
{
    public class KeyboardServices
    {
        public static InlineKeyboardMarkup CreateGetTasksKeyboard()
        {
            var commands = CommandServices.LoadCommandsMenu();
            var menu = new List<List<InlineKeyboardButton>>();
            var currentRow = new List<InlineKeyboardButton>();

            foreach (var command in commands)
            {
                var commandMenu = InlineKeyboardButton.WithCallbackData(command.Name, "command_" + command.Command);
                currentRow.Add(commandMenu);

                if (currentRow.Count == 1)
                {
                    menu.Add(currentRow);
                    currentRow = new List<InlineKeyboardButton>();
                }
            }

            if (currentRow.Count > 0)
            {
                menu.Add(currentRow);
            }

            return new InlineKeyboardMarkup(menu);
        }

        public static InlineKeyboardMarkup CreateDayKeyboard()
        {
            var days = new List<List<InlineKeyboardButton>>();
            var currentRow = new List<InlineKeyboardButton>();

            for (int i = 1; i <= 31; i++)
            {
                var dayButton = InlineKeyboardButton.WithCallbackData(i.ToString(), "day_" + i);
                currentRow.Add(dayButton);

                // Verifica se a linha atual atingiu o limite de 8 botões
                if (currentRow.Count == 8)
                {
                    days.Add(currentRow);
                    currentRow = new List<InlineKeyboardButton>();
                }
            }

            // Adiciona a linha final, se houver botões restantes
            if (currentRow.Count > 0)
            {
                days.Add(currentRow);
            }

            return new InlineKeyboardMarkup(days);
        }

        public static InlineKeyboardMarkup CreateMonthKeyboard(string day)
        {
            var months = new List<List<InlineKeyboardButton>>();
            var currentRow = new List<InlineKeyboardButton>();

            var monthNames = new string[] { "Janeiro", "Fevereiro", "Março", "Abril", "Maio", "Junho", "Julho", "Agosto", "Setembro", "Outubro", "Novembro", "Dezembro" };

            for (int i = 0; i < 12; i++)
            {
                var monthButton = InlineKeyboardButton.WithCallbackData(monthNames[i], "month_" + (i + 1) + $"_{day}");
                currentRow.Add(monthButton);

                // Verifica se a linha atual atingiu o limite de 8 botões
                if (currentRow.Count == 8)
                {
                    months.Add(currentRow);
                    currentRow = new List<InlineKeyboardButton>();
                }
            }

            // Adiciona a linha final, se houver botões restantes
            if (currentRow.Count > 0)
            {
                months.Add(currentRow);
            }

            return new InlineKeyboardMarkup(months);
        }

        public static InlineKeyboardMarkup CreateYearKeyboard(string monthDay)
        {
            var years = new List<List<InlineKeyboardButton>>();
            var currentRow = new List<InlineKeyboardButton>();

            for (int i = 2020; i <= 2023; i++)
            {
                var yearButton = InlineKeyboardButton.WithCallbackData(i.ToString(), "year_" + i + $"_{monthDay}");
                currentRow.Add(yearButton);

                // Verifica se a linha atual atingiu o limite de 8 botões
                if (currentRow.Count == 8)
                {
                    years.Add(currentRow);
                    currentRow = new List<InlineKeyboardButton>();
                }
            }

            // Adiciona a linha final, se houver botões restantes
            if (currentRow.Count > 0)
            {
                years.Add(currentRow);
            }

            return new InlineKeyboardMarkup(years);
        }
    }
}
