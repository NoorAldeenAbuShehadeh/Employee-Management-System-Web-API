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
        public DepartmentController(IDepartmentServices departmentServices)
        {
            _departmentServices = departmentServices;
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> AddDepartment([FromBody] Department department)
        {
            department.ManagerEmail = null;
            if (await _departmentServices.AddDepartment(department))
            {
                return StatusCode(201, new { Message = "department created successfully." });
            }
            return BadRequest(new { Message = "There is an issue with the data" });
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<List<Department>>> GetDepartments()
        {
            var result = await _departmentServices.GetDepartments();
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet("statistics")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<List<Dictionary<string, string>>>> GetDepartmentSatatistics()
        {
            var statistics = await _departmentServices.GetDepartmentSatatistics();
            return Ok(statistics);
        }

        [HttpPut]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> UpdateDepartment([FromBody] Department department)
        {
            if(department == null) return BadRequest();
            bool departmentUpdated = await _departmentServices.UpdateDepartment(department);
            if (departmentUpdated)
            {
                return Ok();
            }
            return BadRequest();
        }
    }
}
