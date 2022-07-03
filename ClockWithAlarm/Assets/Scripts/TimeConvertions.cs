using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    class TimeConvertions
    {
        const int secsInAMin = 60;
        const int secsInAnHour = 60 * secsInAMin;

        public int DateTimeToSeconds(DateTime dateTime)
        {
            return dateTime.Hour * secsInAnHour + dateTime.Minute * secsInAMin + dateTime.Second;
        }
        public DateTime SecondsToDateTime(int seconds)
        {
            DateTime dateTime = new DateTime();
            return dateTime.AddSeconds(seconds);
        }

        public int StringTimeToSeconds(string time)
        {
            int sec = Convert.ToInt32(time.Substring(6, 2));
            int minuts = Convert.ToInt32(time.Substring(3, 2));
            int hours = Convert.ToInt32(time.Substring(0, 2));
            return (sec + minuts * 60 + hours * 3600);
        }
        public string SecondsToStringTime(int time)
        {
            int hours = time / 3600 % 3600;
            int min = time / 60 % 60;
            int sec = time % 60;

            string str = NumberToPartOfTimeString(hours) + ":" + NumberToPartOfTimeString(min) + ":" + NumberToPartOfTimeString(sec);
            return str;
        }
        string NumberToPartOfTimeString(int number)
        {
            string str;
            if (number < 10)
            {
                str = "0" + Convert.ToString(number);
            }
            else
            {
                str = Convert.ToString(number);
            }
            return str;
        }
    }
}
