using System;
using System.Collections.Generic;

namespace ForgettingCurveBot.Model
{
    public class TelegramUser
    {
        public long Id { get; set; }

        public string Nickname { get; set; }

        public Dictionary<DateTimeOffset, string> Messages { get; set; } = new();
        
        public List<CardToRemember> Cards { get; set; } = new();
    }
}
