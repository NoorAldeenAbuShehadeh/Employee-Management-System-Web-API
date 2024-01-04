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
        private ILogger<EmployeeController> _logger;
        public EmployeeController(IEmployeeServices employeeServices, ILogger<EmployeeController> logger)
        {
            _employeeServices = employeeServices;
            _logger = logger;
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> AddEmployee([FromBody] EmployeeCreationModel employeeCreationModel)
        {
            try
            { 
                if (await _employeeServices.AddEmployee(employeeCreationModel))
                {
                    return StatusCode(201, new { Message = "Employee added successfully" });
                }
                return BadRequest(new { Message = "There is an issue with the data" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(EmployeeController)}] - [{nameof(AddEmployee)}] - Error: {ex}");
                throw ex;
            }
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<List<Employee>>> GetEmployees()
        {
            try
            { 
                var result = await _employeeServices.GetEmployees();
                if (result == null) return NotFound();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(EmployeeController)}] - [{nameof(GetEmployees)}] - Error: {ex}");
                throw ex;
            }
        }

        [HttpGet("{DepartmentName}")]
        [Authorize(Roles = "admin,manager")]
        public async Task<ActionResult<List<Employee>>> GetEmployeesInDepartment(string DepartmentName)
        {
            try 
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
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(EmployeeController)}] - [{nameof(GetEmployeesInDepartment)}] - Error: {ex}");
                throw ex;
            }
        }

        [HttpGet("FilterEmployeesBySalary")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<List<Employee>>> FilterEmployeesBySalary([FromQuery] decimal minSalary)
        {
            try
            { 
                var employees = await _employeeServices.FilterEmployeesBySalary(minSalary);
                return Ok(employees);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(EmployeeController)}] - [{nameof(FilterEmployeesBySalary)}] - Error: {ex}");
                throw ex;
            }
        }

        [HttpGet("SearchForEmployeesByCity")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<List<Employee>>> SearchForEmployeesByCity([FromQuery] string city)
        {
            try
            {
                var employees = await _employeeServices.SearchForEmployeesByCity(city);
                return Ok(employees);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(EmployeeController)}] - [{nameof(SearchForEmployeesByCity)}] - Error: {ex}");
                throw ex;
            }
        }

        [HttpPut]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> UpdateEmployee([FromBody] UpdateEmployeeInfo updateEmployeeInfo)
        {
            try
            {
                if (updateEmployeeInfo == null) return BadRequest();
                bool updated = await _employeeServices.UpdateEmployee(updateEmployeeInfo);
                if (updated) return Ok();
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(EmployeeController)}] - [{nameof(UpdateEmployee)}] - Error: {ex}");
                throw ex;
            }
        }

        [HttpGet("GetEmployee")]
        [Authorize]
        public async Task<ActionResult<object>> GetEmployeeInfo()
        {
            try
            {
                var user = HttpContext.Items["User"] as User;
                var (e, u) = await _employeeServices.GetEmployee(user.Email);
                var userInfo = new
                {
                    u.Email,
                    e.DepartmentName,
                    e.PhoneNumber,
                    e.Address,
                    u.Name,
                    u.Role,
                    u.Status
                };
                return Ok(userInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(EmployeeController)}] - [{nameof(GetEmployeeInfo)}] - Error: {ex}");
                throw ex;
            }
        }

        [HttpPut("updateGeneralInfo")]
        [Authorize]
        public async Task<ActionResult> UpdateGeneralEmployeeInfo([FromBody] Employee employee)
        {
            try
            { 
                if (employee == null) return BadRequest();
                var user = HttpContext.Items["User"] as User;
                if (user.Role != "admin")
                {
                    employee.UserEmail = user.Email;
                }
                bool updated = await _employeeServices.UpdateGeneralEmployeeInfo(employee);
                if (updated) return Ok();
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(EmployeeController)}] - [{nameof(UpdateGeneralEmployeeInfo)}] - Error: {ex}");
                throw ex;
            }
        }
    }
}
