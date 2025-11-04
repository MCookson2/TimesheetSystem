using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimesheetSystem.Common.Classes
{
    public class PagedTimesheetEntries
    {
        public IDictionary<int, TimesheetEntry> Entries { get; set; } = new Dictionary<int, TimesheetEntry>();
        public int TotalCount { get; set; }
    }
}
