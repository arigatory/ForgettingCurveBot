using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgettingCurveBot.Models
{
    public class CardToRemember
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Data { get; set; }
        public TypeOfCard Type { get; set; }
        public List<Attempt> Attempts { get; set; }
    }
}
