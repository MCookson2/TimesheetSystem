using TimesheetSystem.Common.Classes;
using TimesheetSystem.Common.Extensions;
using TimesheetSystem.Common.Interfaces;

namespace TimesheetSystem.Common.Services
{
    public class TimesheetService : ITimesheetService
    {
        private readonly Dictionary<int, TimesheetEntry> _entries = new();
        private int _nextId = 0;

        /// <summary>
        /// Adds a new timesheet entry to the collection and assigns it a unique auto-incremented identifier.
        /// Returns success boolean.
        /// </summary>
        /// <param name="entry"></param>
        public bool Add(TimesheetEntry entry)
        {
            bool success = false;

            if (!HasDuplicate(entry))
            {
                _entries.Add(_nextId++, entry);
                success = true;
            }

            return success;
        }

        /// <summary>
        /// Removes the timesheet entry with the specified identifier from the collection, if it exists.
        /// </summary>
        /// <param name="id"></param>
        public void Delete(int id)
        {
            _entries.Remove(id);
        }

        /// <summary>
        /// Updates an existing timesheet entry if the specified ID exists, or adds a new entry with an auto-generated ID if it does not.
        /// Returns success boolean.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="entry"></param>
        public bool Update(int id, TimesheetEntry entry)
        {
            bool success = false;

            if (!HasDuplicate(entry))
            {
                if (_entries.ContainsKey(id))
                {
                    _entries[id] = entry;
                }
                else
                {
                    _entries.Add(_nextId++, entry);
                }

                success = true;
            }

            return success;
        }

        /// <summary>
        /// Retrieves a paginated subset of all timesheet entries in the collection.
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public IDictionary<int, TimesheetEntry> GetAllEntries(int offset = 0, int limit = 10)
        {
            return _entries
                .OrderBy(e => e.Value.Date)
                .Skip(offset)
                .Take(limit)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        /// <summary>
        /// Retrieves all timesheet entries for a specified user, optionally limited to those within the same week as the given date.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public IDictionary<int, TimesheetEntry> GetAllEntriesForUserPerWeek(int userId, DateTime? date)
        {
            var query = BuildEntriesForUserAndWeek(userId, date);

            return query.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        /// <summary>
        /// Calculates the total hours worked per project for a specified user, optionally limited to entries within the same week as the given date.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public IDictionary<int, double> GetHoursPerProject(int userId, DateTime? date)
        {
            var query = BuildEntriesForUserAndWeek(userId, date);

            Dictionary<int, double> hoursPerProject = query
                .Select(kvp => kvp.Value)
                .GroupBy(ts => ts.ProjectID)
                .ToDictionary(p => p.Key, p => p.Sum(ts => ts.HoursWorked));

            return hoursPerProject;
        }

        public int GetTotalCount()
        {
            return _entries.Count();
        }

        private IEnumerable<KeyValuePair<int, TimesheetEntry>> BuildEntriesForUserAndWeek(int userId, DateTime? date)
        {
            var query = _entries
                .Where(kvp => kvp.Value.UserID == userId);

            if (date.HasValue)
            {
                query = query.Where(kvp => kvp.Value.Date.IsInSameWeekAs(date.Value));
            }

            return query;
        }

        private bool HasDuplicate(TimesheetEntry entry)
        {
            return _entries.Any(e => e.Value.UserID == entry.UserID
                && e.Value.ProjectID == entry.ProjectID
                && e.Value.Date.Date == entry.Date.Date);
        }
    }
}
