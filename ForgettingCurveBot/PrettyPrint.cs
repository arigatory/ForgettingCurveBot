using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgettingCurveBot
{
    public static class PrettyPrint
    {
        public static string Progress(int percent)
        {
            string res = "";

            if (percent < 0)
            {
                percent = 0;
            }
            if (percent > 100)
            {
                percent = 100;
            }
            int dec = percent / 10;
            int dig = percent % 10;
            for (int i = 0; i < dec; i++)
            {
                res += "█";
            }
            switch (dig)
            {
                case 1:
                    res += "▂";
                    break;
                case 2:
                    res += "▂";
                    break;
                case 3:
                    res += "▂";
                    break;
                case 4:
                    res += "▂";
                    break;
                case 5:
                    res += "▇";
                    break;
                case 6:
                    res += "▇";
                    break;
                case 7:
                    res += "▇";
                    break;
                case 8:
                    res += "▇";
                    break;
                case 9:
                    res += "▇";
                    break;
                case 0:
                    res += "▁";
                    break;
                default:
                    break;
            }
            int left = 10 - dec - 1;
            for (int i = 0; i < left; i++)
            {
                res += "▁";
            }
            res += $" {percent}%";
            return res;
        }
    }
}
