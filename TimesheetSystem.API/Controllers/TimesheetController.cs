using Microsoft.AspNetCore.Mvc;
using TimesheetSystem.Common.Classes;
using TimesheetSystem.Common.Interfaces;

namespace TimesheetSystem.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TimesheetController : ControllerBase
    {
        private readonly ILogger<TimesheetController> _logger;
        private readonly ITimesheetService _timesheetService;

        public TimesheetController(ILogger<TimesheetController> logger, ITimesheetService timesheetService)
        {
            _logger = logger;
            _timesheetService = timesheetService;
        }

        [HttpGet]
        public ActionResult<PagedTimesheetEntries> GetAllEntries([FromQuery] int offset = 0, [FromQuery] int limit = 10)
        {
            ActionResult result = NotFound();

            try
            {
                Dictionary<int, TimesheetEntry> entries = _timesheetService
                    .GetAllEntries(offset, limit)
                    .ToDictionary();

                int totalCount = _timesheetService.GetTotalCount();

                PagedTimesheetEntries pagedResults = new PagedTimesheetEntries
                {
                    Entries = entries,
                    TotalCount = totalCount
                };

                result = Ok(pagedResults);
            }
            catch (Exception exception)
            {
                _logger.LogError($"TimesheetController::GetAll {exception.Message}");
                result = StatusCode(500);
            }

            return result;
        }

        [HttpGet]
        [Route("entries/{userId}")]
        public ActionResult<IDictionary<int, TimesheetEntry>> GetAllEntriesForUserPerWeek(int userId, [FromQuery] DateTime? date)
        {
            ActionResult result = NotFound();

            try
            {
                Dictionary<int, TimesheetEntry> entries = _timesheetService
                    .GetAllEntriesForUserPerWeek(userId, date)
                    .ToDictionary();

                result = Ok(entries);
            }
            catch (Exception exception)
            {
                _logger.LogError($"TimesheetController::GetAll {exception.Message}");
                result = StatusCode(500);
            }

            return result;
        }

        [HttpGet]
        [Route("projects/{userId}")]
        public ActionResult<IDictionary<int, double>> GetHoursPerProject(int userId, [FromQuery] DateTime? date)
        {
            ActionResult result = NotFound();

            try
            {
                Dictionary<int, double> hoursPerProject = _timesheetService
                    .GetHoursPerProject(userId, date)
                    .ToDictionary();

                result = Ok(hoursPerProject);
            }
            catch (Exception exception)
            {
                _logger.LogError($"TimesheetController::GetAll {exception.Message}");
                result = StatusCode(500);
            }

            return result;
        }

        [HttpPost]
        public ActionResult Add(TimesheetEntry entry)
        {
            ActionResult result = NotFound();

            try
            {
                bool success = _timesheetService.Add(entry);
                result = Ok(success);
            }
            catch (Exception exception)
            {
                _logger.LogError($"TimesheetController::Add {exception.Message}");
                result = StatusCode(500);
            }

            return result;
        }

        [HttpPut]
        public ActionResult Update(int id, TimesheetEntry entry)
        {
            ActionResult result = NotFound();

            try
            {
                bool success = _timesheetService.Update(id, entry);
                result = Ok(success);
            }
            catch (Exception exception)
            {
                _logger.LogError($"TimesheetController::Add {exception.Message}");
                result = StatusCode(500);
            }

            return result;
        }

        [HttpDelete]
        public ActionResult Delete(int id)
        {
            ActionResult result = NotFound();

            try
            {
                _timesheetService.Delete(id);
                result = Ok();
            }
            catch (Exception exception)
            {
                _logger.LogError($"TimesheetController::Add {exception.Message}");
                result = StatusCode(500);
            }

            return result;
        }
    }
}
