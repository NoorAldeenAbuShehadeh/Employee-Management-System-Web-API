using Employee_Management_System.DAL;
using Employee_Management_System.Model;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Employee_Management_System.Services
{
    public class LoginServices : ILoginServices
    {
        private readonly IConfiguration _config;
        private readonly IDUsers _dUsers;
        private readonly ILogger<LoginServices> _logger;
        public LoginServices(IConfiguration config, IDUsers dUsers, ILogger<LoginServices> logger)
        {
            _config = config;
            _dUsers = dUsers;
            _logger = logger;
        }
        public async Task<string> Authenticate(User user)
        {
            try
            {
                string encodedPassword = _dUsers.EncodePassword(user.Password);
                var userData = await _dUsers.LogIn(user.Email, encodedPassword);
                if (userData != null)
                {
                    var token = GenerateToken(userData);
                    return token;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(LoginServices)}] - [{nameof(Authenticate)}] - Error: {ex}");
                throw ex;
            }
        }
        private string GenerateToken(User user)
        {
            try
            {
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
                var claims = new[]
                {
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role)
                };

                var token = new JwtSecurityToken(issuer: _config["Jwt:Issuer"],
                        audience: _config["Jwt:Audience"],
                        claims: claims,
                        expires: DateTime.Now.AddHours(10),
                        signingCredentials: credentials);

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(LoginServices)}] - [{nameof(GenerateToken)}] - Error: {ex}");
                throw ex;
            }
        }
    }
}
