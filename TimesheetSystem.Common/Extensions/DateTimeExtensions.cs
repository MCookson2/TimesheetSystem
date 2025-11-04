using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimesheetSystem.Common.Extensions
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Determines whether the specified<see cref="DateTime"/> instance occurs within the same ISO 8601 week and calendar year
        /// as another <see cref="DateTime"/> value.
        /// </summary>
        /// <param name="date"></param>
        /// <param name="otherDate"></param>
        /// <returns></returns>
        public static bool IsInSameWeekAs(this DateTime date, DateTime otherDate)
        {
            bool result = false;

            if (date.Year == otherDate.Year 
                && ISOWeek.GetWeekOfYear(date) == ISOWeek.GetWeekOfYear(otherDate))
            {
                result = true;
            }

            return result;
        }
    }
}
