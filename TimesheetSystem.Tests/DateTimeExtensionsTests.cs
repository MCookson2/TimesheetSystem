using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimesheetSystem.Common.Extensions;

namespace TimesheetSystem.Tests
{
    public class DateTimeExtensionsTests
    {
        [Fact]
        public void IsInSameWeekAs_SameWeek_ReturnsTrue()
        {
            DateTime monday = new DateTime(2025, 11, 3);
            DateTime thursday = new DateTime(2025, 11, 6);

            bool result = monday.IsInSameWeekAs(thursday);

            Assert.True(result);
        }

        [Fact]
        public void IsInSameWeekAs_DifferentWeek_ReturnsFalse()
        {
            DateTime friday = new DateTime(2025, 11, 7);
            DateTime nextMonday = new DateTime(2025, 11, 10);

            bool result = friday.IsInSameWeekAs(nextMonday);

            Assert.False(result);
        }

        [Fact]
        public void IsInSameWeekAs_SameWeekNumberDifferentYear_ReturnsFalse()
        {
            DateTime date1 = new DateTime(2024, 12, 31);
            DateTime date2 = new DateTime(2025, 1, 1);

            bool result = date1.IsInSameWeekAs(date2);

            Assert.False(result);
        }

        [Fact]
        public void IsInSameWeekAs_DifferentYearsSameISOWeek_ReturnsFalse()
        {
            DateTime dec30 = new DateTime(2024, 12, 30);
            DateTime jan2 = new DateTime(2025, 1, 2);

            bool result = dec30.IsInSameWeekAs(jan2);

            Assert.False(result);
        }
    }
}
