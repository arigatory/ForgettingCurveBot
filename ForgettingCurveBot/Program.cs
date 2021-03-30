using ForgettingCurveBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
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
            string token = System.IO.File.ReadAllText(@"token");
            bot = new TelegramBotClient(token);
            keyboardMarkup = CreateCustomKeyboard();

            bot.OnMessage += Bot_OnMessageReceived;
            bot.OnMessageEdited += Bot_OnMessageReceived;
            bot.OnCallbackQuery += Bot_OnCallbackQuery;
            BotCommand botCommand = new BotCommand { Command = "\\start", Description = "Запускаем бота" };
            
            bot.StartReceiving();
            bot.SetMyCommandsAsync(new List<BotCommand> { botCommand });

            Console.WriteLine("Бот запущен!");
            Console.ReadLine();
        }

        private static async void Bot_OnCallbackQuery(object sender, Telegram.Bot.Args.CallbackQueryEventArgs e)
        {
            var user = new TelegramUser { Id = e.CallbackQuery.Message.Chat.Id, Nickname = e.CallbackQuery.Message.Chat.FirstName };
            user = users[users.IndexOf(user)];
            var cardToRemove = user.Cards.FirstOrDefault(c => c.Title == e.CallbackQuery.Data);
            user.Cards.Remove(cardToRemove);
            await bot.AnswerCallbackQueryAsync(e.CallbackQuery.Id, "Карта удалена");
            await bot.DeleteMessageAsync(e.CallbackQuery.Message.Chat.Id, e.CallbackQuery.Message.MessageId);
        }

        private static async void Bot_OnMessageReceived(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            string input = e.Message.Text;
            string text = $"{DateTime.Now}: {e.Message.Chat.FirstName} {e.Message.Chat.Id} {input}";
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
            switch (input)
            {
                case "/start":
                    await StartCommandAsync(user.Id);
                    break;
                case "/stop":
                    await StopCommandAsync(user.Id);
                    break;
                case "Показать все карточки":
                    await ShowAllCards(user);
                    break;
                case "Статистика":
                    await ShowStatistics(user);
                    break;
                default:
                    await AddNewCardAsync(user, input);
                    break;
            }
        }

        private static Task ShowStatistics(TelegramUser user)
        {
            throw new NotImplementedException();
        }

        private static async Task StopCommandAsync(long id)
        {
            users.First(u => u.Id == id).Cards.Clear();
            await bot.SendTextMessageAsync(id, $"Остановка бота! Все карточки удалены", replyMarkup: keyboardMarkup);
        }

        private static async Task ShowAllCards(TelegramUser user)
        {
            RemoveUnfilledCards(user);
            if (user.Cards.Count > 0)
            {
                foreach (var card in user.Cards)
                {
                    InlineKeyboardButton inlineKeyboardButtonDelete = new InlineKeyboardButton { CallbackData = $"{card.Title}", Text = "Удалить" };
                    InlineKeyboardButton inlineKeyboardButtonView = new InlineKeyboardButton { CallbackData = $"{card.Title}", Text = "Посмотреть" };
                    InlineKeyboardButton inlineKeyboardButtonDone = new InlineKeyboardButton { CallbackData = $"{card.Title}", Text = "Изучена" };
                    InlineKeyboardButton[] inlineKeyboardButtons = new InlineKeyboardButton[] { inlineKeyboardButtonDelete, inlineKeyboardButtonView, inlineKeyboardButtonDone };
                    InlineKeyboardMarkup inlineKeyboardMarkup = new InlineKeyboardMarkup(inlineKeyboardButtons);
                    await bot.SendTextMessageAsync(user.Id, card.Data, replyMarkup: inlineKeyboardMarkup, parseMode: ParseMode.Markdown);
                }
            } 
            else
            {
                await bot.SendTextMessageAsync(user.Id, "Нет ни одной карточки");
            }

        }

        private static void RemoveUnfilledCards(TelegramUser user)
        {
            user.Cards.RemoveAll(c => string.IsNullOrEmpty(c.Title));
        }

        private static async Task AddNewCardAsync(TelegramUser user, string input)
        {
            if (user.Cards.Count==0)
            {
                await CreateNewCard(user, input);
            }
            else
            {
                var lastCard = user.Cards?.Last();
                if (string.IsNullOrEmpty(lastCard.Title))
                {
                    await AddTitleForNewCard(lastCard, input, user);

                }
                else
                {
                    await CreateNewCard(user, input);
                }
            }
        }

        private static async Task AddTitleForNewCard(CardToRemember lastCard, string input, TelegramUser user)
        {
            lastCard.Title = input;
            string text = $"Создана карточка\n*{lastCard.Title}*\n{lastCard.Data}\n";
            await bot.SendTextMessageAsync(user.Id, text, replyMarkup: keyboardMarkup, parseMode: ParseMode.Markdown);
        }

        private static async Task CreateNewCard(TelegramUser user, string input)
        {
            string text = $"Создание карточки с текстом '{input}'\n Введите название для карточки";
            CardToRemember card = new CardToRemember { Data = input, Type = TypeOfCard.Text };
            user.Cards.Add(card);
            await bot.SendTextMessageAsync(user.Id, text, replyMarkup: keyboardMarkup);
        }

        private static async System.Threading.Tasks.Task StartCommandAsync(long id)
        {
            await bot.SendTextMessageAsync(id, $"Добро пожаловать в суперсовременную систему запоминания! Для того чтобы начать, просто отправьте боту то, что хотите запомнить!", replyMarkup: keyboardMarkup);
        }

        private static ReplyKeyboardMarkup CreateCustomKeyboard()
        {
            var rkm = new ReplyKeyboardMarkup();
            rkm.Keyboard = new KeyboardButton[][]
            {
                new KeyboardButton[]
                {
                    new KeyboardButton("Показать все карточки")
                },

                new KeyboardButton[]
                {
                    new KeyboardButton("Топ-1"),
                    new KeyboardButton("Топ-3"),
                    new KeyboardButton("Топ-5")
                },
                new KeyboardButton[]
                {
                    new KeyboardButton("Обучаться"),
                    new KeyboardButton("Статистика")
                }
            };
            return rkm;
        }
    }
}
