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
        private readonly ILogger<LoginController> _logger;
        public LoginController(ILoginServices loginServices, ILogger<LoginController> logger)
        {
            _loginServices = loginServices;
            _logger = logger;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<string>> Login([FromBody] User userLogin)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(LoginController)}] - [{nameof(Login)}] - Error: {ex}");
                throw ex;
            }
        }

    }
}