using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ForgettingCurveBot.Model
{
    public class TelegramUser
    {
        public long Id { get; set; }

        [Required]
        public long TelegramIdentification { get; set; }

        [Required]
        [MaxLength(50)]
        public string Nickname { get; set; }

        public Dictionary<DateTimeOffset, string> Messages { get; set; } = new();
        
        public List<CardToRemember> Cards { get; set; } = new();

        public short? NotificationIntervalId { get; set; }

        public NotificationInterval NotificationInterval { get; set; }
    }
}
