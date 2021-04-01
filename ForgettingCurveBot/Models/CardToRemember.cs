using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ForgettingCurveBot.Models
{
    public class CardToRemember
    {
        static int nextId;
        public int Id { get; private set; }
        public CardToRemember()
        {
            Id = Interlocked.Increment(ref nextId);
        }
        public string Title { get; set; }
        public string Data { get; set; }
        public TypeOfCard Type { get; set; }
        public List<Attempt> Attempts { get; set; }
    }
}
