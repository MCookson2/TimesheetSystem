using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimesheetSystem.Common.Classes;
using TimesheetSystem.Common.Extensions;
using TimesheetSystem.Common.Services;

namespace TimesheetSystem.Tests
{
    public class TimesheetServiceTests
    {
        #region Add
        [Fact]
        public void Add_ValidEntry_ReturnsTrueAndAddsEntry()
        {
            TimesheetService service = new TimesheetService();
            TimesheetEntry entry = new TimesheetEntry { UserID = 1, ProjectID = 10, HoursWorked = 5, Date = DateTime.Today };

            bool result = service.Add(entry);

            Assert.True(result);
            Assert.Single(service.GetAllEntries());
            Assert.Contains(entry, service.GetAllEntries().Values);
        }

        [Fact]
        public void Add_IncrementsId()
        {
            TimesheetService service = new TimesheetService();
            TimesheetEntry firstEntry = new TimesheetEntry { UserID = 1, ProjectID = 20, HoursWorked = 5, Date = DateTime.Now };
            TimesheetEntry secondEntry = new TimesheetEntry { UserID = 2, ProjectID = 10, HoursWorked = 15, Date = DateTime.Now };

            service.Add(firstEntry);
            service.Add(secondEntry);

            var entries = service.GetAllEntries();

            Assert.Equal(2, entries.Count);
            Assert.True(entries.ContainsKey(0));
            Assert.True(entries.ContainsKey(1));
            Assert.Same(firstEntry, entries[0]);
            Assert.Same(secondEntry, entries[1]);
        }

        [Fact]
        public void Add_DuplicateEntry_ReturnsFalseAndDoesNotAdd()
        {
            TimesheetService service = new TimesheetService();
            TimesheetEntry entry = new TimesheetEntry { UserID = 1, ProjectID = 10, HoursWorked = 5, Date = DateTime.Today };

            service.Add(entry);
            bool result = service.Add(entry);

            Assert.False(result);
            Assert.Single(service.GetAllEntries());
        }
        #endregion

        #region Delete
        [Fact]
        public void Delete_RemovesEntryFromDictionary()
        {
            TimesheetEntry entryToDelete = new TimesheetEntry { UserID = 1, ProjectID = 10, HoursWorked = 8 };
            TimesheetEntry entryToKeep = new TimesheetEntry { UserID = 2, ProjectID = 5, HoursWorked = 1 };

            TimesheetService service = new TimesheetService();

            service.Add(entryToDelete);
            service.Add(entryToKeep);

            service.Delete(0);

            var entries = service.GetAllEntries();

            Assert.Single(entries);
            Assert.False(entries.ContainsKey(0));
            Assert.True(entries.ContainsKey(1));
        }

        [Fact]
        public void Delete_NonExistentId_DoesNotRemoveAnyEntries()
        {
            TimesheetEntry entry1 = new TimesheetEntry { UserID = 1, ProjectID = 10, HoursWorked = 8 };
            TimesheetEntry entry2 = new TimesheetEntry { UserID = 2, ProjectID = 5, HoursWorked = 1 };

            var service = new TimesheetService();

            service.Add(entry1);
            service.Add(entry2);

            service.Delete(999);

            var entries = service.GetAllEntries();

            Assert.Equal(2, entries.Count);
            Assert.False(entries.ContainsKey(999));
            Assert.True(entries.ContainsKey(0));
            Assert.True(entries.ContainsKey(1));
        }
        #endregion

        #region Update
        [Fact]
        public void Update_ExistingEntry_ReturnsTrueAndReplacesValue()
        {
            TimesheetService service = new TimesheetService();
            TimesheetEntry original = new TimesheetEntry { UserID = 1, ProjectID = 10, HoursWorked = 4, Date = DateTime.Today };
            TimesheetEntry updated = new TimesheetEntry { UserID = 1, ProjectID = 10, HoursWorked = 8, Date = DateTime.Today.AddDays(1) };

            service.Add(original);
            bool result = service.Update(0, updated);

            Assert.True(result);
            Assert.Single(service.GetAllEntries());
            Assert.Same(updated, service.GetAllEntries()[0]);
        }

        [Fact]
        public void Update_NonExistentId_AddsNewEntryAndReturnsTrue()
        {
            TimesheetService service = new TimesheetService();
            TimesheetEntry entry = new TimesheetEntry { UserID = 1, ProjectID = 10, HoursWorked = 6, Date = DateTime.Today };

            bool result = service.Update(99, entry);

            Assert.True(result);
            Assert.Single(service.GetAllEntries());
            Assert.Same(entry, service.GetAllEntries().Values.First());
        }

        [Fact]
        public void Update_DuplicateEntry_ReturnsFalseAndDoesNotChangeCollection()
        {
            TimesheetService service = new TimesheetService();
            TimesheetEntry entry1 = new TimesheetEntry { UserID = 1, ProjectID = 10, HoursWorked = 5, Date = DateTime.Today };
            TimesheetEntry entry2 = new TimesheetEntry { UserID = 1, ProjectID = 10, HoursWorked = 5, Date = DateTime.Today };

            service.Add(entry1);
            bool result = service.Update(0, entry2);

            Assert.False(result);
            Assert.Single(service.GetAllEntries());
            Assert.Same(entry1, service.GetAllEntries()[0]);
        }
        #endregion

        #region GetAllEntries
        [Fact]
        public void GetAllEntries_WithOffsetAndLimit_ReturnsPaginatedSubset()
        {
            TimesheetService service = new TimesheetService();

            for (int i = 1; i <= 10; i++)
            {
                service.Add(new TimesheetEntry
                {
                    UserID = 1,
                    ProjectID = 10,
                    HoursWorked = i,
                    Date = DateTime.Today.AddDays(i)
                });
            }

            var result = service.GetAllEntries(offset: 3, limit: 4);

            Assert.Equal(4, result.Count);
            Assert.Equal(new[] { 3, 4, 5, 6 }, result.Keys);
            Assert.Equal(4, result.Values.First().HoursWorked);
        }

        [Fact]
        public void GetAllEntries_OffsetNearEnd_ReturnsRemainingEntries()
        {
            TimesheetService service = new TimesheetService();

            for (int i = 1; i <= 10; i++)
            {
                service.Add(new TimesheetEntry
                {
                    UserID = 1,
                    ProjectID = 10,
                    HoursWorked = i,
                    Date = DateTime.Today.AddDays(i)
                });
            }

            var result = service.GetAllEntries(offset: 8, limit: 5);

            Assert.Equal(2, result.Count);
            Assert.Equal(new[] { 8, 9 }, result.Keys);
            Assert.Equal(9, result.Values.First().HoursWorked);
        }

        [Fact]
        public void GetAllEntries_OffsetBeyondRange_ReturnsEmpty()
        {
            TimesheetService service = new TimesheetService();

            for (int i = 1; i <= 5; i++)
            {
                service.Add(new TimesheetEntry 
                { 
                    UserID = 1, 
                    ProjectID = 10, 
                    HoursWorked = i 
                });
            }

            var result = service.GetAllEntries(offset: 10, limit: 5);

            Assert.Empty(result);
        }

        [Fact]
        public void GetAllEntries_EmptyDictionary_ReturnsEmpty()
        {
            TimesheetService service = new TimesheetService();

            var result = service.GetAllEntries();

            Assert.Empty(result);
        }
        #endregion

        #region GetAllEntriesForUserPerWeek
        [Fact]
        public void GetAllEntriesForUserPerWeek_WithoutDate_FiltersByUserId()
        {
            TimesheetService service = new TimesheetService();

            DateTime today = DateTime.Today;

            service.Add(new TimesheetEntry { UserID = 1, ProjectID = 10, HoursWorked = 8, Date = today });
            service.Add(new TimesheetEntry { UserID = 2, ProjectID = 20, HoursWorked = 5, Date = today });
            service.Add(new TimesheetEntry { UserID = 1, ProjectID = 30, HoursWorked = 6, Date = today });

            var result = service.GetAllEntriesForUserPerWeek(userId: 1, date: null);

            Assert.Equal(2, result.Count);
            Assert.All(result.Values, e => Assert.Equal(1, e.UserID));
        }

        [Fact]
        public void GetAllEntriesForUserPerWeek_WithDate_FiltersByUserAndWeek()
        {
            TimesheetService service = new TimesheetService();

            DateTime referenceDate = new DateTime(2025, 11, 5);

            service.Add(new TimesheetEntry { UserID = 1, ProjectID = 10, HoursWorked = 8, Date = referenceDate });
            service.Add(new TimesheetEntry { UserID = 1, ProjectID = 10, HoursWorked = 6, Date = referenceDate.AddDays(-2) });
            service.Add(new TimesheetEntry { UserID = 1, ProjectID = 10, HoursWorked = 5, Date = referenceDate.AddDays(10) });
            service.Add(new TimesheetEntry { UserID = 2, ProjectID = 20, HoursWorked = 9, Date = referenceDate });

            var result = service.GetAllEntriesForUserPerWeek(userId: 1, date: referenceDate);

            Assert.Equal(2, result.Count);
            Assert.All(result.Values, e => Assert.Equal(1, e.UserID));
            Assert.All(result.Values, e => Assert.True(e.Date.IsInSameWeekAs(referenceDate)));
        }

        [Fact]
        public void GetAllEntriesForUserPerWeek_NoMatchingEntries_ReturnsEmpty()
        {
            TimesheetService service = new TimesheetService();
            DateTime date = DateTime.Today;

            service.Add(new TimesheetEntry { UserID = 2, ProjectID = 10, HoursWorked = 5, Date = date });

            IDictionary<int, TimesheetEntry> result = service.GetAllEntriesForUserPerWeek(1, date);

            Assert.Empty(result);
        }
        #endregion

        #region GetHoursPerProject
        [Fact]
        public void GetHoursPerProject_WithoutDate_GroupsAndSumsByProjectForUser()
        {
            TimesheetService service = new TimesheetService();
            DateTime today = DateTime.Today;

            service.Add(new TimesheetEntry { UserID = 1, ProjectID = 10, HoursWorked = 5, Date = today });
            service.Add(new TimesheetEntry { UserID = 1, ProjectID = 10, HoursWorked = 3, Date = today.AddDays(1) });
            service.Add(new TimesheetEntry { UserID = 1, ProjectID = 20, HoursWorked = 4, Date = today.AddDays(2) });
            service.Add(new TimesheetEntry { UserID = 2, ProjectID = 10, HoursWorked = 9, Date = today.AddDays(3) });

            IDictionary<int, double> result = service.GetHoursPerProject(1, null);

            Assert.Equal(2, result.Count);
            Assert.Equal(8, result[10]);
            Assert.Equal(4, result[20]);
            Assert.False(result.ContainsKey(999));
        }

        [Fact]
        public void GetHoursPerProject_WithDate_FiltersByWeekAndSumsCorrectly()
        {
            TimesheetService service = new TimesheetService();
            DateTime referenceDate = new DateTime(2025, 11, 5);

            service.Add(new TimesheetEntry { UserID = 1, ProjectID = 10, HoursWorked = 4, Date = referenceDate });
            service.Add(new TimesheetEntry { UserID = 1, ProjectID = 10, HoursWorked = 6, Date = referenceDate.AddDays(-2) });
            service.Add(new TimesheetEntry { UserID = 1, ProjectID = 20, HoursWorked = 3, Date = referenceDate.AddDays(-1) });
            service.Add(new TimesheetEntry { UserID = 1, ProjectID = 10, HoursWorked = 8, Date = referenceDate.AddDays(7) });
            service.Add(new TimesheetEntry { UserID = 2, ProjectID = 10, HoursWorked = 10, Date = referenceDate });

            IDictionary<int, double> result = service.GetHoursPerProject(1, referenceDate);

            Assert.Equal(2, result.Count);
            Assert.Equal(10, result[10]);
            Assert.Equal(3, result[20]);
        }

        [Fact]
        public void GetHoursPerProject_NoMatchingEntries_ReturnsEmpty()
        {
            TimesheetService service = new TimesheetService();
            DateTime today = DateTime.Today;

            service.Add(new TimesheetEntry { UserID = 2, ProjectID = 99, HoursWorked = 10, Date = today });

            IDictionary<int, double> result = service.GetHoursPerProject(1, today);

            Assert.Empty(result);
        }
        #endregion
    }
}
