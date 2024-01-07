using Employee_Management_System.Model;
using Employee_Management_System.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Employee_Managment_System_web_API.Controllers
{
    [Route("api/departments")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private IDepartmentServices _departmentServices;
        private ILogger<DepartmentController> _logger;
        public DepartmentController(IDepartmentServices departmentServices, ILogger<DepartmentController> logger)
        {
            _departmentServices = departmentServices;
            _logger = logger;
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> AddDepartment([FromBody] Department department)
        {
            try
            {
                department.ManagerEmail = null;
                if (await _departmentServices.AddDepartment(department))
                {
                    return StatusCode(201, new { Message = "department created successfully." });
                }
                return BadRequest(new { Message = "There is an issue with the data" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(DepartmentController)}] - [{nameof(AddDepartment)}] - Error: {ex}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<List<Department>>> GetDepartments()
        {
            try
            {
                var result = await _departmentServices.GetDepartments();
                if (result == null) return NotFound();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(DepartmentController)}] - [{nameof(GetDepartments)}] - Error: {ex}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("statistics")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<List<Dictionary<string, string>>>> GetDepartmentSatatistics()
        {
            try
            {
                var statistics = await _departmentServices.GetDepartmentSatatistics();
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(DepartmentController)}] - [{nameof(GetDepartmentSatatistics)}] - Error: {ex}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPut]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> UpdateDepartment([FromBody] Department department)
        {
            try
            {
                if (department == null) return BadRequest();
                bool departmentUpdated = await _departmentServices.UpdateDepartment(department);
                if (departmentUpdated)
                {
                    return Ok();
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(DepartmentController)}] - [{nameof(UpdateDepartment)}] - Error: {ex}");
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}
