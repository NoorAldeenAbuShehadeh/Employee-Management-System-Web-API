using Employee_Management_System.Model;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

namespace Employee_Management_System.DAL
{
    public class DUsers : IDUsers
    {
        private readonly AppDbContext _context;
        private readonly ILogger<DUsers> _logger;
        public DUsers(AppDbContext context, ILogger<DUsers> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<bool> AddUser(User user)
        {
            try
            {
                if (user == null)
                {
                    _logger.LogError($"[{nameof(DUsers)}] - [{nameof(AddUser)}] - Bad Request should has a user to add it");
                    return false;
                }
                else
                {
                    ValidateUser(user);
                    UserDTO userDTO = new UserDTO()
                    {
                        Email = user.Email,
                        Name = user.Name,
                        Password = user.Password,
                        Role = user.Role,
                        Status = user.Status
                    };
                    await _context.Users.AddAsync(userDTO);
                    _logger.LogInformation($"[{nameof(DUsers)}] - [{nameof(AddUser)}] - Added new user: {user.Email}");
                    return true;
                }
            }
            catch (ValidationException ex)
            {
                _logger.LogError($"[{nameof(DUsers)}] - [{nameof(AddUser)}] - Validation Error while add new user: {ex}");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(DUsers)}] - [{nameof(AddUser)}] - Error while add new user: {ex}");
                throw ex;
            }
        }
        public async Task<List<User>>? GetUsers()
        {
            try
            {
                var users = await _context.Users
                    .Select(u => new User
                    {
                        Email = u.Name,
                        Name = u.Name,
                        Role = u.Role,
                    })
                    .ToListAsync();
                _logger.LogInformation($"[{nameof(DUsers)}] - [{nameof(GetUsers)}] - Retrived all users");
                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(DUsers)}] - [{nameof(GetUsers)}] - Error while retrive all departments: {ex.Message}");
                throw ex;
            }
        }
        public async Task<User>? GetUser(string email)
        {
            try
            {
                UserDTO? userDTO = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (userDTO == null)
                {
                    _logger.LogError($"[{nameof(DUsers)}] - [{nameof(GetUser)}] - User with email {email} not found.");
                    return null;
                }
                User user = new User
                {
                    Email = userDTO.Email,
                    Name = userDTO.Name,
                    Role = userDTO.Role,
                    Password = userDTO.Password,
                    Status = userDTO.Status,
                };
                _logger.LogInformation($"[{nameof(DUsers)}] - [{nameof(GetUser)}] - Get data for user");
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(DUsers)}] - [{nameof(GetUser)}] - Error while get user: {ex}");
                throw ex;
            }
        }
        public async Task<bool> DeleteUser(string email)
        {
            try
            {
                UserDTO? userDTO = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (userDTO == null)
                {
                    _logger.LogError($"[{nameof(DUsers)}] - [{nameof(DeleteUser)}] - User with email {email} not found.");
                    return false;
                }
                else
                {
                    userDTO.Status = "inActive";
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"[{nameof(DUsers)}] - [{nameof(DeleteUser)}] - User with email {email} deleted.");
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(DUsers)}] - [{nameof(DeleteUser)}] - Error while delete user: {ex}");
                throw ex;
            }
        }
        public async Task<User> LogIn(string email, string password)
        {
            try
            {
                UserDTO? userDTO = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (userDTO == null)
                {
                    _logger.LogError($"[{nameof(DUsers)}] - [{nameof(LogIn)}] - User with email {email} not found.");
                    return null;
                }
                else
                {
                    string userPassword = userDTO.Password;
                    if (userPassword == password)
                    {
                        User user = new User()
                        {
                            Email = userDTO.Email,
                            Role = userDTO.Role,
                            Name = userDTO.Name,
                            Status = userDTO.Status,
                            Password = userDTO.Password
                        };
                        _logger.LogInformation($"[{nameof(DUsers)}] - [{nameof(LogIn)}] - User with email {email} logged in.");
                        return user;
                    }
                    else
                    {
                        _logger.LogError($"[{nameof(DUsers)}] - [{nameof(LogIn)}] - User with email {email} try to logg in but enter wrong password.");
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(DUsers)}] - [{nameof(LogIn)}] - Error while update user: {ex}");
                return null;
            }
        }
        public async Task<bool> UpdateUser(User user)
        {
            try
            {
                UserDTO? userDTO = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
                if (userDTO == null)
                {
                    _logger.LogError($"[{nameof(DUsers)}] - [{nameof(UpdateUser)}] - User with email {user?.Email} not found.");
                    return false;
                }
                else
                {
                    ValidateUser(user);
                    userDTO.Password = user.Password;
                    userDTO.Name = user.Name;
                    userDTO.Role = user.Role;
                    _logger.LogInformation($"[{nameof(DUsers)}] - [{nameof(UpdateUser)}] - User with email {userDTO.Email} updated.");
                    return true;
                }
            }
            catch (ValidationException ex)
            {
                _logger.LogError($"[{nameof(DUsers)}] - [{nameof(UpdateUser)}] - Validation Error while update user: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(DUsers)}] - [{nameof(UpdateUser)}] - Error while update user: {ex.Message}");
                return false;
            }
        }
        private void ValidateUser(User user)
        {
            var validationContext = new ValidationContext(user, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();

            if (!Validator.TryValidateObject(user, validationContext, validationResults, validateAllProperties: true))
            {
                var errorMessages = validationResults.Select(result => result.ErrorMessage);
                throw new ValidationException(string.Join(Environment.NewLine, errorMessages));
            }
        }
        public string? EncodePassword(string password)
        {
            try
            {
                byte[] bytes = Encoding.Unicode.GetBytes(password);
                byte[] inArray = HashAlgorithm.Create("SHA1").ComputeHash(bytes);
                return Convert.ToBase64String(inArray);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(DUsers)}] - [{nameof(EncodePassword)}] - Error while encoding password: {ex.Message}");
                return null;
            }
        }
    }
}
