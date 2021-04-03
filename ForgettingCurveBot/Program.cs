using ForgettingCurveBot.Data;
using ForgettingCurveBot.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace ForgettingCurveBot
{
    class Program
    {
        private static TelegramBotClient bot;
        private static ReplyKeyboardMarkup keyboardMarkup;
        private static DataProvider _cardDataProvider = new();
        

        static void Main(string[] args)
        {
            string token = System.IO.File.ReadAllText(@"token");
            bot = new TelegramBotClient(token);
            keyboardMarkup = CreateCustomKeyboard();

            bot.OnMessage += Bot_OnMessageReceived;
            bot.OnMessageEdited += Bot_OnMessageReceived;
            bot.OnCallbackQuery += Bot_OnCallbackQuery;

            //TODO: Узнать зачем?
            //BotCommand botCommand = new BotCommand { Command = "\\start", Description = "Запускаем бота2" };
            //bot.SetMyCommandsAsync(new List<BotCommand> { botCommand });

            bot.StartReceiving();

            Console.WriteLine("Бот запущен!");
            Console.ReadLine();
        }

        private static async void Bot_OnCallbackQuery(object sender, Telegram.Bot.Args.CallbackQueryEventArgs args)
        {
            long userId = args.CallbackQuery.Message.Chat.Id;
            var user = _cardDataProvider.LoadTelegramUser(userId);

            // Id of the tapped card
            int cardId;
            if(!int.TryParse(args.CallbackQuery.Data.Split(':')[1],out cardId))
            {
                Debug.WriteLine($"Could'n parse card with data {args.CallbackQuery.Data}");
            }
            

            // What user wants to do with the card
            // All cards have data like "d:23849893" where 23849893 is card's Id;
            string shortCommandForCard = args.CallbackQuery.Data.Split(':')[0];
            switch (shortCommandForCard)
            {
                case "d":
                    var cardToRemove = user.Cards.FirstOrDefault(c => c.Id == cardId);
                    if (cardToRemove != null)
                    {
                        string cardTitle = cardToRemove.Title;
                        user.Cards.Remove(cardToRemove);
                        await bot.AnswerCallbackQueryAsync(args.CallbackQuery.Id, $"Карта '{cardTitle}' удалена");
                        await bot.DeleteMessageAsync(userId, args.CallbackQuery.Message.MessageId);
                    }
                    else
                    {
                        await bot.AnswerCallbackQueryAsync(args.CallbackQuery.Id, $"Такой карты уже, похоже, нет");
                        await bot.DeleteMessageAsync(userId, args.CallbackQuery.Message.MessageId);
                    }
                    break;
                case "v":
                    try
                    {
                        await ShowPretyCardAsync(args, user, cardId);
                    }
                    catch (Exception ex)
                    {
                        await bot.SendTextMessageAsync(user.Id, "Не получилось выполнить предыдущую команду", replyMarkup: keyboardMarkup);
                        Console.Write("Не получилось выполнить команду: ");
                        Console.WriteLine(ex.Message);
                    }
                    break;
                default:
                    break;
            }
            _cardDataProvider.SaveTelegramUser(user);
        }

        private static async Task ShowPretyCardAsync(CallbackQueryEventArgs e, TelegramUser user, int cardId)
        {
            var cardToShow = user.Cards.FirstOrDefault(c => c.Id == cardId);
            if (cardToShow == null)
            {
                await bot.AnswerCallbackQueryAsync(e.CallbackQuery.Id, $"Такой карты уже, похоже, нет");
                await bot.DeleteMessageAsync(user.Id, e.CallbackQuery.Message.MessageId);
                return;
            }

            InlineKeyboardMarkup inlineKeyboardMarkup = new InlineKeyboardMarkup
            (
                   new InlineKeyboardButton[] {
                        new InlineKeyboardButton { CallbackData = $"d:{cardToShow.Id}", Text = "Удалить" },
                        new InlineKeyboardButton { CallbackData = $"r:{cardToShow.Id}", Text = "Сбросить" },
                        new InlineKeyboardButton { CallbackData = $"l:{cardToShow.Id}", Text = "Изучена" }
                    }
            );


            await bot.AnswerCallbackQueryAsync(e.CallbackQuery.Id);
            await bot.SendTextMessageAsync(user.Id,
                $"*{cardToShow.Title}*\n--------\n{cardToShow.Data}",
                replyMarkup: inlineKeyboardMarkup, parseMode: ParseMode.Markdown);
        }

        private static async void Bot_OnMessageReceived(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            var user = _cardDataProvider.LoadTelegramUser(e.Message.Chat.Id);
            string input = e.Message.Text;
            string text = $"Time:{DateTime.Now}\tName:{e.Message.Chat.FirstName}\tId:{e.Message.Chat.Id}\tType:{e.Message.Type}\tMessage:{input}";
            
            System.IO.File.AppendAllText("data.log", $"{text}\n");
            user.Messages[DateTimeOffset.Now] = text;

            Console.WriteLine(text);

            switch (input)
            {
                case "/start":
                    await StartCommandAsync(user);
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
            _cardDataProvider.SaveTelegramUser(user);
        }

        private static async Task ShowStatistics(TelegramUser user)
        {
            await bot.SendTextMessageAsync(user.Id, $"Всего карточек: {user.Cards.Count}", replyMarkup: keyboardMarkup);
        }

        private static async Task StopCommandAsync(long id)
        {   
            //TODO DeleteCardsMaybe
            await bot.SendTextMessageAsync(id, $"Остановка бота! Все карточки удалены", replyMarkup: keyboardMarkup);
        }

        private static async Task ShowAllCards(TelegramUser user)
        {
            RemoveUnfilledCards(user);
            if (user.Cards.Count > 0)
            {
                foreach (var card in user.Cards)
                {
                    InlineKeyboardButton[] inlineKeyboardButtons = new InlineKeyboardButton[] {
                        new InlineKeyboardButton { CallbackData = $"f:{card.Id}", Text = "🤷‍♂️‍" },
                        new InlineKeyboardButton { CallbackData = $"v:{card.Id}", Text = "👀" },
                        new InlineKeyboardButton { CallbackData = $"r:{card.Id}", Text = "✅" }
                    };
                    InlineKeyboardMarkup inlineKeyboardMarkup = new InlineKeyboardMarkup(inlineKeyboardButtons);

                    await bot.SendTextMessageAsync(user.Id, $"{card.Title}\n{PrettyPrint.Progress(card.Progress())}", replyMarkup: inlineKeyboardMarkup, parseMode: ParseMode.Markdown);
                }
            } 
            else
            {
                await bot.SendTextMessageAsync(user.Id, "Нет ни одной карточки");
            }

        }

        private static async Task ShowOneCard(TelegramUser user, int cardId)
        {
            RemoveUnfilledCards(user);
            if (user.Cards.Count > 0)
            {
                foreach (var card in user.Cards)
                {
                    InlineKeyboardButton[] inlineKeyboardButtons = new InlineKeyboardButton[] {
                        new InlineKeyboardButton { CallbackData = $"{card.Title}", Text = "Удалить" },
                        new InlineKeyboardButton { CallbackData = $"{card.Title}", Text = "Посмотреть" },
                        new InlineKeyboardButton { CallbackData = $"{card.Title}", Text = "Помню" }
                    };
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
                await StartCreatingNewCard(user, input);
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
                    await StartCreatingNewCard(user, input);
                }
            }
        }

        private static async Task AddTitleForNewCard(CardToRemember lastCard, string input, TelegramUser user)
        {
            lastCard.Title = input;
            lastCard.Id = user.Cards.Max(c => c.Id) + 1;
            string text = $"Создана карточка:\n*{lastCard.Title}*\n{lastCard.Data}\n";
            await bot.SendTextMessageAsync(user.Id, text, replyMarkup: keyboardMarkup, parseMode: ParseMode.Markdown);
        }

        /// <summary>
        /// Adds new card without a Title. The next user's message will be the Title, if it's not a command
        /// </summary>
        /// <param name="user"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        private static async Task StartCreatingNewCard(TelegramUser user, string input)
        {
            string text = $"Создание карточки с текстом '{input}'\n Введите название для карточки";
            CardToRemember card = new CardToRemember { Data = input, Type = TypeOfCard.Text };
            user.Cards.Add(card);
            await bot.SendTextMessageAsync(user.Id, text, replyMarkup: keyboardMarkup);
        }

        private static async Task StartCommandAsync(TelegramUser user)
        {
            await bot.SendTextMessageAsync(user.Id, $"Добро пожаловать в суперсовременную систему запоминания! Для того чтобы начать, просто отправьте боту то, что хотите запомнить!", replyMarkup: keyboardMarkup);
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
