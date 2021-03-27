using ForgettingCurveBot.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace ForgettingCurveBot
{
    class Program
    {
        private static TelegramBotClient bot;
        private static List<TelegramUser> users = new();
        private static ReplyKeyboardMarkup keyboardMarkup;
        

        static void Main(string[] args)
        {
            string token = System.IO.File.ReadAllText(@"C:\tokens\ForgettingCurveBot.txt");
            bot = new TelegramBotClient(token);
            keyboardMarkup = CreateCustomKeyboard();

            bot.OnMessage += Bot_OnMessageReceived;
            bot.OnMessageEdited += Bot_OnMessageReceived;

            BotCommand botCommand = new BotCommand { Command = "\\start", Description = "Запускаем бота" };
            bot.SetMyCommandsAsync(new List<BotCommand> { botCommand });

            bot.StartReceiving();
            Console.WriteLine("Бот запущен!");
            Console.ReadLine();
        }

        private static async void Bot_OnMessageReceived(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            string text = $"{DateTime.Now}: {e.Message.Chat.FirstName} {e.Message.Chat.Id} {e.Message.Text}";
            System.IO.File.AppendAllText("data.log", $"{text}\n");
            var user = new TelegramUser { Id = e.Message.Chat.Id, Nickname = e.Message.Chat.FirstName };
            if (!users.Contains(user))
            {
                users.Add(user);
            }
            else
            {
                user = users[users.IndexOf(user)];
            }
            user.Messages[DateTimeOffset.Now] = text;
            Console.WriteLine(text);
            Console.WriteLine(e.Message.Type);
            await bot.SendTextMessageAsync(user.Id, $"Вы отправили всего сообщений:{user.Messages.Count}", replyMarkup: keyboardMarkup);
        }

        private static ReplyKeyboardMarkup CreateCustomKeyboard()
        {
            var rkm = new ReplyKeyboardMarkup();
            rkm.Keyboard = new KeyboardButton[][]
            {
                new KeyboardButton[]
                {
                    new KeyboardButton("1-1"),
                    new KeyboardButton("1-2")
                },

                new KeyboardButton[]
                {
                    new KeyboardButton("2")
                },

                new KeyboardButton[]
                {
                    new KeyboardButton("3-1"),
                    new KeyboardButton("3-2"),
                    new KeyboardButton("3-3")
                }
            };
            return rkm;
        }
    }
}
