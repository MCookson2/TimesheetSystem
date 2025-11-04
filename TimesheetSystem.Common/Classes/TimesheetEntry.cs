namespace TimesheetSystem.Common.Classes
{
    public class TimesheetEntry
    {
        public int UserID { get; set; }
        public int ProjectID { get; set; }
        public DateTime Date { get; set; }
        public double HoursWorked { get; set; }
        public string? Description { get; set; }

        public int Year => Date.Year;
        public int Month => Date.Month;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public TimesheetEntry()
        {
        }
    }
}
