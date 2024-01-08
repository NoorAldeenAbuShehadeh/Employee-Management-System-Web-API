using Employee_Management_System.Model;
using Employee_Management_System.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Employee_Managment_System_web_API.Controllers
{
    [Route("api/salaries")]
    [ApiController]
    public class SalaryController : ControllerBase
    {
        private ISalaryServices _salaryServices;
        private ILogger<SalaryController> _logger;
        public SalaryController(ISalaryServices salaryServices, ILogger<SalaryController> logger)
        {
            _salaryServices = salaryServices;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<List<Dictionary<string, string>>>> GetEmployeeSalaries()
        {
            try
            {
                var result = await _salaryServices.GetEmployeeSalaries();
                if (result == null) return NotFound();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(SalaryController)}] - [{nameof(GetEmployeeSalaries)}] - Error: {ex}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("GetEmployeeSalaryDetails")]
        [Authorize]
        public async Task<ActionResult<Salary>> GetEmployeeSalaryDetails([FromQuery] string? employeeEmail)
        {
            try
            {
                var user = HttpContext.Items["User"] as User;
                var result = await _salaryServices.GetEmployeeSalaryDetails(user, employeeEmail);
                if (result == null) return BadRequest();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(SalaryController)}] - [{nameof(GetEmployeeSalaryDetails)}] - Error: {ex}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPut]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> UpdateSalary([FromBody] Salary salary)
        {
            try
            {
                if (salary == null) return BadRequest();
                bool salaryUpdated = await _salaryServices.UpdateSalary(salary);
                if (salaryUpdated) return Ok();
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(SalaryController)}] - [{nameof(UpdateSalary)}] - Error: {ex}");
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}
