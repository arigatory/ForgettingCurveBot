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
        private static ReplyKeyboardMarkup keyboardMarkup = CreateCustomKeyboard();
        private static DataProvider _cardDataProvider = new();
        

        static void Main(string[] args)
        {
            string token = System.IO.File.ReadAllText(@"token");
            bot = new TelegramBotClient(token);

            bot.OnMessage += Bot_OnMessageReceived;
            bot.OnMessageEdited += Bot_OnMessageReceived;
            bot.OnCallbackQuery += Bot_OnCallbackQuery;

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

            string text = $"Time:{DateTime.Now}\tName:{args.CallbackQuery.Message.Chat.FirstName}\tId:{userId}\tType:query\tMessage:{args.CallbackQuery.Data}";

            System.IO.File.AppendAllText("data.log", $"{text}\n");
            user.Messages[DateTimeOffset.Now] = text;

            Console.WriteLine(text);

            // What user wants to do with the card
            // All cards have data like "d:23849893" where 23849893 is card's Id;
            string shortCommandForCard = args.CallbackQuery.Data.Split(':')[0];
            switch (shortCommandForCard)
            {
                case Codes.Delete:
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
                case Codes.View:
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
                case Codes.Forgot:
                case Codes.Remember:
                case Codes.Restore:
                    try
                    {
                        await bot.AnswerCallbackQueryAsync(args.CallbackQuery.Id, $"Пока не реализовано");
                    }
                    catch (Exception ex)
                    {
                        await bot.SendTextMessageAsync(user.Id, "Функционал для данной кнопки пока не реализован", replyMarkup: keyboardMarkup);
                        Console.Write("Попытка выполнить нереализованную команду: ");
                        Console.WriteLine(ex.Message);
                        throw;
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
                        new InlineKeyboardButton { CallbackData = $"{Codes.Remember}:{cardToShow.Id}", Text = "Помню" },
                        new InlineKeyboardButton { CallbackData = $"{Codes.Forgot}:{cardToShow.Id}", Text = "Не помню" },
                        new InlineKeyboardButton { CallbackData = $"{Codes.Delete}:{cardToShow.Id}", Text = "Удалить" }
                    }
            );


            await bot.AnswerCallbackQueryAsync(e.CallbackQuery.Id);
            await bot.SendTextMessageAsync(user.Id,
                $"*{cardToShow.Title}*\n--------\n{cardToShow.Data}",
                replyMarkup: inlineKeyboardMarkup, parseMode: ParseMode.Markdown);
        }

        private static async void Bot_OnMessageReceived(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            var user = _cardDataProvider.LoadTelegramUser(e.Message.Chat.Id, e.Message.Chat.FirstName);
            string input = e.Message.Text;
            string text = $"Time:{DateTime.Now}\tName:{e.Message.Chat.FirstName}\tId:{e.Message.Chat.Id}\tType:{e.Message.Type}\tMessage:{input}";
            
            System.IO.File.AppendAllText("data.log", $"{text}\n");
            user.Messages[DateTimeOffset.Now] = text;

            Console.WriteLine(text);
            try
            {
                switch (input)
                {
                    case "/start":

                        await StartCommandAsync(user);
                        break;
                    case "/stop":
                        await StopCommandAsync(user.Id);
                        break;
                    case TextCommands.ShowAll:
                        await ShowAllCards(user);
                        break;
                    case TextCommands.ShowActual:
                    case TextCommands.ShowDeleted:
                    case TextCommands.TurnNotificationsOn:
                    case TextCommands.TurnNotificationsOff:
                        await ShowNotRealized(user);
                        break;
                    case TextCommands.Statistics:
                        await ShowStatistics(user);
                        break;
                    default:
                        await AddNewCardAsync(user, input);
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
            _cardDataProvider.SaveTelegramUser(user);
        }

        private static async Task ShowStatistics(TelegramUser user)
        {
            await bot.SendTextMessageAsync(user.Id, $"Всего карточек: {user.Cards.Count}", replyMarkup: keyboardMarkup);
        }

        private static async Task ShowNotRealized(TelegramUser user)
        {
            await bot.SendTextMessageAsync(user.Id, $"Данная кнопка пока не работает", replyMarkup: keyboardMarkup);
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
                        new InlineKeyboardButton { CallbackData = $"{Codes.Forgot}:{card.Id}", Text = "🤷‍♂️‍" },
                        new InlineKeyboardButton { CallbackData = $"{Codes.View}:{card.Id}", Text = "👀" },
                        new InlineKeyboardButton { CallbackData = $"{Codes.Delete}:{card.Id}", Text = "🗑️" },
                        new InlineKeyboardButton { CallbackData = $"{Codes.Remember}:{card.Id}", Text = "✅" }
                    };
                    InlineKeyboardMarkup inlineKeyboardMarkup = new InlineKeyboardMarkup(inlineKeyboardButtons);

                    await bot.SendTextMessageAsync(user.Id, $"{card.Title}\n`------------------------------`\n_(текст карточки скрыт,\nпосмотреть 👀?)_\n{PrettyPrint.Progress(card.Progress())}", replyMarkup: inlineKeyboardMarkup, parseMode: ParseMode.Markdown);
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

            await bot.SendTextMessageAsync(user.Id, $"Добро пожаловать в суперсовременную систему запоминания! Для того чтобы начать, просто отправьте боту то, что хотите запомнить!\n\nМы добавили для вас несколько учебных карточек, чтобы Вам было проще понять как работает бот.", replyMarkup: keyboardMarkup);
            await AddTutorialCards(user);
          
        }

        private static async Task AddTutorialCards(TelegramUser user)
        {
            int maxId = 1;
            if (user.Cards.Count > 0)
            {
                maxId = user.Cards.Max(c => c.Id);
            }
            CardToRemember[] cards = new CardToRemember[]
            {
                new CardToRemember
                {
                    Id = Interlocked.Increment(ref maxId),
                    Title = "Как добавить карточку?",
                    Data = "Нет ничего проще! Нужно просто прислать боту сообщение. Затем бот попросит ввести заголовок для дарточки. Мы советуем писать заголовок в виде вопроса, но это не обязательно."
                },
                new CardToRemember
                {
                    Id = Interlocked.Increment(ref maxId),
                    Title = "Как работает бот?",
                    Data = "Бот использует кривую Эббингауза для того, чтобы напоминать Вам о том, что Вы хотели бы запомнить, как можно реже, но в то же время, когда Вы, скорее всего, еще помните. Бот также обучается и совершенствуется, поэтому он может не придерживаться строгих правил, а напоминать некоторым пользователям чаще, если им запоминание дается сложнее."
                },new CardToRemember
                {
                    Id = Interlocked.Increment(ref maxId),
                    Title = "Что показывает прогресс?",
                    Data = "Прогресс показывает насколько карточка хорошо запомнена. Не пытайтесь получить прогресс 100% за один день. Для того, чтобы запомнить надолго, требуется в среднем 2-3 месяца, а для запоминания на всю жизнь - около 2х лет."
                }
            };
            foreach (var card in cards)
            {
                user.Cards.Add(card);
            }
            await ShowAllCards(user);
        }

        private static ReplyKeyboardMarkup CreateCustomKeyboard()
        {
            var rkm = new ReplyKeyboardMarkup();
            rkm.Keyboard = new KeyboardButton[][]
            {
                new KeyboardButton[]
                {
                    new KeyboardButton(TextCommands.ShowAll),
                    new KeyboardButton(TextCommands.ShowActual),
                    new KeyboardButton(TextCommands.ShowDeleted)
                },

                new KeyboardButton[]
                {
                    new KeyboardButton(TextCommands.TurnNotificationsOn),
                    new KeyboardButton(TextCommands.TurnNotificationsOff),
                },
                new KeyboardButton[]
                {
                    new KeyboardButton(TextCommands.Statistics)
                }
            };
            return rkm;
        }
    }
}
