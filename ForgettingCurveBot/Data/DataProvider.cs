using ForgettingCurveBot.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ForgettingCurveBot.Data
{
    public class DataProvider
    {
        // Reading file will have 3 attemps because it's likely to be failed if many users use the app
        private const int _numberOfRetries = 3;
        private const int _delayOnRetry = 1000;

        /// <summary>
        /// Never return null, if no eser exists this method creates a new one. Nickname can be added later if needed. It can be updated with every call.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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
            

        public void SaveTelegramUser(TelegramUser telegramUser)
        {
            string _userFileName = $"user{telegramUser.Id}.json";

            for (int i = 1; i <= _numberOfRetries; ++i)
            {
                try
                {
                    var json = JsonConvert.SerializeObject(telegramUser, Formatting.Indented);
                    File.WriteAllText(_userFileName, json);
                    Debug.WriteLine($"Сохранены данные пользователя {telegramUser.Id}: {telegramUser.Nickname}");
                    break;
                }
                catch (IOException e) when (i <= _numberOfRetries)
                {
                    Console.WriteLine(e.Message);
                    Thread.Sleep(_delayOnRetry);
                }
            }
        }

        public IEnumerable<TelegramUser> LoadAllUsers()
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

    }
}
