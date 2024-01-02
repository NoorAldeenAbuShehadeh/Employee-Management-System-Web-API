using Employee_Management_System.DAL;
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
        public LeaveController(ILeaveServices leaveServices) 
        {
            _leaveServices = leaveServices;
        }

        [HttpPost]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult> AddLeave([FromBody] Leave leave)
        {
            var user = HttpContext.Items["User"] as User;
            leave.EmployeeEmail = user.Email;
            leave.Status = LeaveStatus.Pending.ToString();
            bool leaveAdded = await _leaveServices.AddLeave(leave);
            if (leaveAdded)
                return StatusCode(201, new { Message = "leave added successfully" });
            return BadRequest(new { Message = "There is an issue with the data" });
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<List<Attendance>>> GetLeaves()
        {
            var result = await _leaveServices.GetLeaves();
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet("leave-trend")] 
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<List<Dictionary<string, string>>>> LeaveTrend()
        {
            var result = await _leaveServices.LeaveTrend();
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet("{departmentName}")]
        [Authorize(Roles = "admin,manager")]
        public async Task<ActionResult<List<Leave>>> GetDepartmentLeaves(string departmentName)
        {
            var user = HttpContext.Items["User"] as User;
            var (leaves, authorized) = await _leaveServices.GetDepartmentLeaves(user, departmentName);
            if(!authorized) return Unauthorized();
            return Ok(leaves);
        }

        [HttpGet("{departmentName}/pendingLeaves")]
        [Authorize(Roles = "admin,manager")]
        public async Task<ActionResult<List<Leave>>> GetPendingLeavesForDepartment(string departmentName)
        {
            if (departmentName == null) return BadRequest();
            var user = HttpContext.Items["User"] as User;
            var (leaves, authorized) = await _leaveServices.GetPendingLeavesForDepartment(user, departmentName);
            if (!authorized) return Unauthorized();
            if (leaves == null) return NotFound();
            return Ok(leaves);
        }

        [HttpGet("GetleavesForEmployee")]
        [Authorize]
        public async Task<ActionResult<List<Leave>>> GetLeavesForEmployee([FromQuery] string? employeeEmail)
        {
            var user = HttpContext.Items["User"] as User;
            var (leaves, authorized) = await _leaveServices.GetLeavesForEmployee(user, employeeEmail);
            if (authorized)
            {
                return Ok(leaves);
            }
            return Unauthorized();
        }

    }
}
