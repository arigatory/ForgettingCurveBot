using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgettingCurveBot.Models
{
    public class Attempt
    {
        public int Id { get; set; }
        public DateTimeOffset Date { get; set; }
        public bool Correct { get; set; }
    }
}
