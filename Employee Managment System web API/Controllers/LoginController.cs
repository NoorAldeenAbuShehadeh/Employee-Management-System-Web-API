using Employee_Management_System.Model;
using Employee_Management_System.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Employee_Managment_System_web_API.Controllers
{
    [Route("api/login")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ILoginServices _loginServices;
        public LoginController(ILoginServices loginServices)
        {
            _loginServices = loginServices;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<string>> Login([FromBody] User userLogin)
        {
            if (userLogin == null)
            {
                return BadRequest();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var token = await _loginServices.Authenticate(userLogin);
            if (token != null)
            {
                return Ok(new { token = token });
            }
            return Unauthorized("Wrong email and/or password");
        }

    }
}