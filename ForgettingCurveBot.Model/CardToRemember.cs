using System;
using System.Collections.Generic;

namespace ForgettingCurveBot.Model
{
    public class CardToRemember
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Data { get; set; }
        public TypeOfCard Type { get; set; }
        public List<Attempt> Attempts { get; set; } = new();
        public bool Learned { get; set; } = false;
        public bool Deleted { get; set; } = false;

        //TODO: make it work
        public int Progress()
        {
            Random r = new Random();
            return r.Next(101);
        }
    }
}