using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgettingCurveBot.Models
{
    public class TelegramUser : IEquatable<TelegramUser>
    {
        public string Nickname { get; set; }
        public long Id { get; set; }
        public Dictionary<DateTimeOffset, string> Messages { get; } = new();
        public List<CardToRemember> Cards { get; } = new();

        public bool Equals(TelegramUser other) => other.Id == this.Id;
    }
}
