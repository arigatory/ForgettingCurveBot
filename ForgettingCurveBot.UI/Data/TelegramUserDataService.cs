using ForgettingCurveBot.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace ForgettingCurveBot.UI.Data
{
    public class TelegramUserDataService : ITelegramUserDataService
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
