using Employee_Management_System.Model;
using Employee_Management_System.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Employee_Managment_System_web_API.Controllers
{
    [Route("api/leaves")]
    public class LeaveController : ControllerBase
    {
        private ILeaveServices _leaveServices;
        private ILogger<LeaveController> _logger;
        public LeaveController(ILeaveServices leaveServices, ILogger<LeaveController> logger)
        {
            _leaveServices = leaveServices;
            _logger = logger;
        }

        [HttpPost]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult> AddLeave([FromBody] Leave leave)
        {
            try
            {
                var user = HttpContext.Items["User"] as User;
                leave.EmployeeEmail = user.Email;
                leave.Status = LeaveStatus.Pending.ToString();
                bool leaveAdded = await _leaveServices.AddLeave(leave);
                if (leaveAdded)
                    return Ok();
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(LeaveController)}] - [{nameof(AddLeave)}] - Error: {ex}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<List<Attendance>>> GetLeaves()
        {
            try
            {
                var result = await _leaveServices.GetLeaves();
                if (result == null) return NotFound();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(LeaveController)}] - [{nameof(GetLeaves)}] - Error: {ex}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("leave-trend")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<List<Dictionary<string, string>>>> LeaveTrend()
        {
            try
            {
                var result = await _leaveServices.LeaveTrend();
                if (result == null) return NotFound();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(LeaveController)}] - [{nameof(LeaveTrend)}] - Error: {ex}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("{departmentName}")]
        [Authorize(Roles = "admin,manager")]
        public async Task<ActionResult<List<Leave>>> GetDepartmentLeaves(string departmentName)
        {
            try
            {
                var user = HttpContext.Items["User"] as User;
                var (leaves, authorized) = await _leaveServices.GetDepartmentLeaves(user, departmentName);
                if (!authorized) return Unauthorized();
                if (leaves == null) return NotFound();
                return Ok(leaves);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(LeaveController)}] - [{nameof(GetDepartmentLeaves)}] - Error: {ex}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("{departmentName}/pendingLeaves")]
        [Authorize(Roles = "admin,manager")]
        public async Task<ActionResult<List<Leave>>> GetPendingLeavesForDepartment(string departmentName)
        {
            try
            { 
                if (departmentName == null) return BadRequest();
                var user = HttpContext.Items["User"] as User;
                var (leaves, authorized) = await _leaveServices.GetPendingLeavesForDepartment(user, departmentName);
                if (!authorized) return Unauthorized();
                if (leaves == null) return NotFound();
                return Ok(leaves);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(LeaveController)}] - [{nameof(GetPendingLeavesForDepartment)}] - Error: {ex}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("GetleavesForEmployee")]
        [Authorize]
        public async Task<ActionResult<List<Leave>>> GetLeavesForEmployee([FromQuery] string? employeeEmail)
        {
            try
            {
                var user = HttpContext.Items["User"] as User;
                var (leaves, authorized) = await _leaveServices.GetLeavesForEmployee(user, employeeEmail);
                if (authorized)
                {
                    if(leaves == null) return BadRequest();
                    return Ok(leaves);
                }
                return Unauthorized();
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(LeaveController)}] - [{nameof(AddLeave)}] - Error: {ex}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPut]
        [Authorize(Roles = "admin,manager")]
        public async Task<ActionResult> UpdateLeaveStatus([FromBody] Leave leave)
        {
            try
            {
                if (leave == null) return BadRequest();
                bool updated = await _leaveServices.UpdateLeaveStatus(leave);
                if (updated) return Ok();
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(LeaveController)}] - [{nameof(UpdateLeaveStatus)}] - Error: {ex}");
                return StatusCode(500, "Internal Server Error");
            }
        }

    }
}
