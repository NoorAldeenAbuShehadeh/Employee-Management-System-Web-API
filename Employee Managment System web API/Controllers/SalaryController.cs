using Employee_Management_System.DAL;
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
        public SalaryController(ISalaryServices salaryServices)
        {
            _salaryServices = salaryServices;
        }

        [HttpGet]
        [Authorize(Roles ="admin")]
        public async Task<ActionResult<List<Dictionary<string, string>>>> GetEmployeeSalaries()
        {
            var result = await _salaryServices.GetEmployeeSalaries();
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet("{EmployeeEmail}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<Salary>> GetEmployeeSalaryDetails(string employeeEmail)
        {
            var result = await _salaryServices.GetEmployeeSalaryDetails(employeeEmail);
            if (result == null) return NotFound();
            return Ok(result);
        }
    }
}
