using StarCorpBot.Models;
using System.Xml;
using Telegram.Bot.Types;

namespace StarCorpBot.Services
{
    public class CommandServices
    {
        public static List<BotCommand> LoadCommands()
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Commands.resx");

            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);

            XmlNodeList dataNodes = doc.SelectNodes("//root/data");

            var commands = new List<BotCommand>();

            foreach (XmlNode dataNode in dataNodes)
            {
                string name = dataNode.Attributes["name"].Value;
                string value = dataNode.SelectSingleNode("value").InnerText;
                string comment = null;

                XmlNode commentNode = dataNode.SelectSingleNode("comment");
                if (commentNode != null)
                {
                    comment = commentNode.InnerText;
                }

                commands.Add(new BotCommand
                {
                    Command = value,
                    Description = comment,
                });
            }

            return commands;
        }
        public static List<BotCommandModel> LoadCommandsMenu()
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "CommandsMenu.resx");

            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);

            XmlNodeList dataNodes = doc.SelectNodes("//root/data");

            var commands = new List<BotCommandModel>();

            foreach (XmlNode dataNode in dataNodes)
            {
                string name = dataNode.Attributes["name"].Value;
                string value = dataNode.SelectSingleNode("value").InnerText;
                string comment = null;

                XmlNode commentNode = dataNode.SelectSingleNode("comment");
                if (commentNode != null)
                {
                    comment = commentNode.InnerText;
                }

                commands.Add(new BotCommandModel
                {
                    Name = name,
                    Command = value,
                    Description = comment,
                });
            }

            return commands;
        }
    }
}
