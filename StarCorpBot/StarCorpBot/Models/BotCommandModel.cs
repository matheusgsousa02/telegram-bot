using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace StarCorpBot.Models
{
    public class BotCommandModel : BotCommand
    {
        public string Name { get; set; }
    }
}
