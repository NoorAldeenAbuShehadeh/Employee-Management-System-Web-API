using Employee_Management_System.Model;
using Employee_Management_System.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Employee_Managment_System_web_API.Controllers
{
    [Route("api/employees")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private IEmployeeServices _employeeServices;
        public EmployeeController(IEmployeeServices employeeServices)
        {
            _employeeServices = employeeServices;
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> AddEmployee([FromBody] EmployeeCreationModel employeeCreationModel)
        {
            if (await _employeeServices.AddEmployee(employeeCreationModel))
            {
                return StatusCode(201, new { Message = "Employee added successfully" });
            }
            return BadRequest(new { Message = "There is an issue with the data" });
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<List<Employee>>> GetEmployees()
        {
            var result = await _employeeServices.GetEmployees();
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet("{DepartmentName}")]
        [Authorize(Roles = "admin,manager")]
        public async Task<ActionResult<List<Employee>>> GetEmployeesInDepartment(string DepartmentName)
        {
            var user = HttpContext.Items["User"] as User;
            var (result, authorized) = await _employeeServices.GetEmployeesInDepartment(user, DepartmentName);
            if (!authorized)
            {
                return StatusCode(403, new { Message = "Access denied. You don't has permission." });
            }
            if (result == null) return NotFound();
            return Ok(result);
        }

    }
}
