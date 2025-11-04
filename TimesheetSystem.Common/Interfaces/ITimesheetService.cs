using TimesheetSystem.Common.Classes;

namespace TimesheetSystem.Common.Interfaces
{
    public interface ITimesheetService
    {
        IDictionary<int, TimesheetEntry> GetAllEntries(int offset = 0, int limit = 10);
        IDictionary<int, TimesheetEntry> GetAllEntriesForUserPerWeek(int userId, DateTime? week);
        IDictionary<int, double> GetHoursPerProject(int userId, DateTime? week);
        int GetTotalCount();
        bool Add(TimesheetEntry entry);
        bool Update(int id, TimesheetEntry entry);
        void Delete(int id);
    }
}
