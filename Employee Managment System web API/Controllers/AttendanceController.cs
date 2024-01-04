using Employee_Management_System.Model;
using Employee_Management_System.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Employee_Managment_System_web_API.Controllers
{
    [Route("api/attendances")]
    [ApiController]
    public class AttendanceController : ControllerBase
    {
        private IAttendanceServices _attendanceServices;
        private ILogger<AttendanceController> _logger;
        public AttendanceController(IAttendanceServices attendanceServices, ILogger<AttendanceController> logger)
        {
            _attendanceServices = attendanceServices;
            _logger = logger;
        }

        [HttpPost]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<string>> AddAttendance([FromBody] Attendance attendance)
        {
            try
            {
                if (attendance == null)
                {
                    return BadRequest();
                }
                var user = HttpContext.Items["User"] as User;
                bool added = await _attendanceServices.AddAttendance(attendance, user);
                if (added)
                    return StatusCode(201, new { Message = "attendance added successfully" });
                return BadRequest(new { Message = "There is an issue with the data" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(AttendanceController)}] - [{nameof(AddAttendance)}] - Error: {ex}");
                throw ex;
            }
        }

        [HttpGet("{DepartmentName}")]
        [Authorize(Roles = "admin,manager")]
        public async Task<ActionResult<List<Attendance>>> GetAttendancesForDepartment(string DepartmentName, [FromQuery] DateTime? startDate = null)
        {
            try
            {
                if (DepartmentName == null)
                {
                    return BadRequest();
                }
                var user = HttpContext.Items["User"] as User;
                var (result, authorized) = await _attendanceServices.GetAttendancesForDepartment(DepartmentName, startDate, user);
                if (!authorized) return StatusCode(403, new { Message = "Access denied. You don't has permission." });
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(AttendanceController)}] - [{nameof(GetAttendancesForDepartment)}] - Error: {ex}");
                throw ex;
            }
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<List<Attendance>>> GetAttenadanceReport([FromQuery] DateTime startDate)
        {
            try
            {
                var attendances = await _attendanceServices.AttendancesReport(startDate);
                return Ok(attendances);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(AttendanceController)}] - [{nameof(GetAttenadanceReport)}] - Error: {ex}");
                throw ex;
            }
        }

        [HttpGet("GetAttendanceForEmployee")]
        [Authorize]
        public async Task<ActionResult<List<Attendance>>> GetAttendanceForEmployee([FromQuery] string? employeeEmail, [FromQuery] DateTime startDate)
        {
            try
            {
                var user = HttpContext.Items["User"] as User;
                var (attendances, authorized) = await _attendanceServices.GetAttendanceForEmployee(user, employeeEmail, startDate);
                if (authorized)
                {
                    return Ok(attendances);
                }
                return Unauthorized();
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(AttendanceController)}] - [{nameof(AddAttendance)}] - Error: {ex}");
                throw ex;
            }
        }
    }
}
