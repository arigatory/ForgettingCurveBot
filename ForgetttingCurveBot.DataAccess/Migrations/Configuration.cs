namespace ForgetttingCurveBot.DataAccess.Migrations
{
    using ForgettingCurveBot.Model;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading;

    internal sealed class Configuration : DbMigrationsConfiguration<ForgetttingCurveBot.DataAccess.ForgettingCurveBotDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ForgetttingCurveBot.DataAccess.ForgettingCurveBotDbContext context)
        {
            var service = new TelegramUserDataService();
            var users = service.GetAll();
            foreach (var item in users)
            {
                if (string.IsNullOrEmpty(item.Nickname))
                {
                    item.Nickname = "?";
                }
                Console.WriteLine($"{item.Id}\t{item.Nickname}\t{item}");
            }
            context.Users.AddOrUpdate(u => u.TelegramIdentification, users.ToArray());
            context.NotificationIntervals.AddOrUpdate(nt => nt.IntervalMinutes,
                new NotificationInterval { Name = "Каждую минуту", IntervalMinutes=1 },
                new NotificationInterval { Name = "Каждые  15 минут", IntervalMinutes = 15 },
                new NotificationInterval { Name = "Каждый час", IntervalMinutes = 60 });
        }
    }

    public class TelegramUserDataService
    {
        private static readonly int _numberOfRetries = 3;
        private static readonly int _delayOnRetry = 1000;

        public IEnumerable<TelegramUser> GetAll()
        {
            var users = new List<TelegramUser>();
            Regex reg = new Regex(@"user([0-9]+)\.json$");
            var files = Directory.GetFiles(Environment.CurrentDirectory, "*.json").Where(path => reg.IsMatch(path));
            foreach (var file in files)
            {
                long userId = long.Parse(reg.Match(file).Groups[1].ToString());
                users.Add(LoadTelegramUser(userId));
            }

            return users;
        }

        public TelegramUser LoadTelegramUser(long id, string nickname = "")
        {
            string _userFileName = $"user{id}.json";

            TelegramUser telegramUser = null;

            if (!File.Exists(_userFileName))
            {
                telegramUser = new TelegramUser
                {
                    Id = id,
                    TelegramIdentification = id,
                    Cards = new List<CardToRemember>(),
                    Messages = new Dictionary<DateTimeOffset, string>(),
                    Nickname = nickname
                };
            }
            else
            {

                for (int i = 1; i <= _numberOfRetries; ++i)
                {
                    try
                    {
                        var json = File.ReadAllText(_userFileName);
                        telegramUser = JsonConvert.DeserializeObject<TelegramUser>(json);
                        telegramUser.Nickname = nickname;
                        telegramUser.TelegramIdentification = telegramUser.Id;
                        break;
                    }
                    catch (IOException e) when (i <= _numberOfRetries)
                    {
                        Console.WriteLine(e.Message);
                        Thread.Sleep(_delayOnRetry);
                    }
                }
            }
            return telegramUser;
        }
    }
}
