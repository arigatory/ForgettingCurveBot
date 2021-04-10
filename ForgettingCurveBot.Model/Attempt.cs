using System;

namespace ForgettingCurveBot.Model
{
    public class Attempt
    {
        public int Id { get; set; }
        public DateTimeOffset Date { get; set; }
        public bool Correct { get; set; }
    }
}