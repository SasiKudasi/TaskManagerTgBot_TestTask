using System;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TaskManagerTgBot
{
    internal class Program
    {
        private static Dictionary<User, List<Tasks>> _dictionary = new Dictionary<User, List<Tasks>>();
        private static User _user = new User(123, "msg");

        static void Main(string[] args)
        {
            // var token = System.IO.File.ReadAllText("token.txt");
            var bot = new TelegramBotClient("6968966018:AAFHGeMDIXZlUJOHnya9eZzHbXylxJ3Z_Ms");

            var receiver = new ReceiverOptions
            {
                AllowedUpdates = new UpdateType[] { },
            };
            bot.StartReceiving(updateHandler: Handler, pollingErrorHandler: ErrorHandler, receiverOptions: receiver);
            Console.ReadLine();
        }

        private static async Task Handler(ITelegramBotClient client, Update update, CancellationToken token)
        {
            _user.Id = update.Message!.Chat.Id;
            _user.Message = update.Message.Text!;
            if (!_dictionary.ContainsKey(_user))
            {
                _dictionary.Add(_user, new List<Tasks>());
            }

            if (_user.Message == "/start")
            {
                await SendMenu(client, token);
            }
            else if (_user.Message.StartsWith("/add"))
            {
                await AddNewTask(client, token);
            }
            else if (_user.Message == "/list")
            {
                await ShowAllTasks(client, token);

            }
            else if (_user.Message.StartsWith("/done"))
            {
                await SwitchTaskStatus(client, token);
            }
            else
            {
                await client.SendTextMessageAsync(_user.Id,
                    "I dont know this command, ssory!",
                    cancellationToken: token);
            }
        }

        private static async Task SwitchTaskStatus(ITelegramBotClient client, CancellationToken token)
        {
            int index = 0;
            string pattern = @"^/done\s+(\d+)$";
            Match match = Regex.Match(_user.Message, pattern);
            if (match.Success)
            {
                index = int.Parse(match.Groups[1].Value);
            }
            if (index <= 0 || index > _dictionary[_user].Count)
                await client.SendTextMessageAsync(_user.Id,
                    "Enter a valid number - /done <Task Number>",
                    cancellationToken: token);
            else
            {
                _dictionary[_user].Find(x => x.TaskId == index)!.Status = "Done";

                await client.SendTextMessageAsync(_user.Id,
               $"The task with number {index} has been successfully changed to Done",
               cancellationToken: token);
            }
        }

        private static async Task ShowAllTasks(ITelegramBotClient client, CancellationToken token)
        {
            foreach (var task in _dictionary[_user])
            {
                await client.SendTextMessageAsync(_user.Id,
                    $"Task Number: {task.TaskId}.\nDescription:\n{task.Description}\n" +
                    $"Task Status: {task.Status}",
                    cancellationToken: token);
            }
        }

        private static async Task AddNewTask(ITelegramBotClient client, CancellationToken token)
        {
            string pattern = @"^/add\s+(.*)";
            Match match = Regex.Match(_user.Message, pattern);

            if (match.Groups[1].Value != "")
            {
                _dictionary[_user].Add(new Tasks()
                {
                    TaskId = _dictionary[_user].Count + 1,
                    Description = match.Groups[1].Value,
                    Status = "ToDo"
                });

                await client.SendTextMessageAsync(_user.Id,
                   $"The Task with number: {_dictionary[_user].Count},\n" +
                   $"Task Description:\n" +
                   $"{_dictionary[_user].Find(x => x.TaskId == _dictionary[_user].Count)!.Description}\n" +
                   $"Successfully added",
                   cancellationToken: token);
            }
            else
            {
                await client.SendTextMessageAsync(_user.Id,
                   $"Enter a task /add <Task Description>" ,
                   cancellationToken: token);
            }

        }

        private static async Task SendMenu(ITelegramBotClient client, CancellationToken token)
        {
            await client.SendTextMessageAsync(_user.Id,
                "Hello!\nTo add a task - write /add\n" +
                "To show Your task list - write /list\n" +
                "To change the task status - write /done",
                cancellationToken: token);
        }


        private static Task ErrorHandler(ITelegramBotClient client, Exception exception, CancellationToken token)
        {
            Console.WriteLine($"{exception.Message}");
            return Task.CompletedTask;
        }


    }
}
