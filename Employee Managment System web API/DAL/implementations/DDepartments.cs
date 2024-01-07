using Employee_Management_System.Model;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Employee_Management_System.DAL
{
    public class DDepartments : IDDepartments
    {
        private readonly AppDbContext _context;
        private readonly ILogger<DDepartments> _logger;

        public DDepartments(AppDbContext context, ILogger<DDepartments> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> AddDepartment(Department department)
        {
            try
            {
                if (department == null)
                {
                    _logger.LogError($"[{nameof(DDepartments)}] - [{nameof(AddDepartment)}] - Bad Request should has a department to add it");
                    return false;
                }
                else
                {
                    ValidateDepartment(department);
                    DepartmentDTO departmentDTO = new DepartmentDTO()
                    {
                        Name = department.Name,
                        ManagerEmail = department.ManagerEmail,
                    };
                    await _context.Departments.AddAsync(departmentDTO);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"[{nameof(DDepartments)}] - [{nameof(AddDepartment)}] - Added new Department: {department.Name}");
                    return true;
                }
            }
            catch (ValidationException ex)
            {
                _logger.LogError($"[{nameof(DDepartments)}] - [{nameof(AddDepartment)}] - Validation Error while add new department: {ex}");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(DDepartments)}] - [{nameof(AddDepartment)}] - Error while add new department: {ex}");
                throw ex;
            }
        }
        public async Task<List<Department>> GetDepartments()
        {
            try
            {
                var departmentDTOs = await _context.Departments
                    .Select(d => new Department
                    {
                        ManagerEmail = d.ManagerEmail,
                        Name = d.Name,
                    })
                    .ToListAsync();
                _logger.LogInformation($"[{nameof(DDepartments)}] - [{nameof(GetDepartments)}] - Retrived all departments");
                return departmentDTOs;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(DDepartments)}] - [{nameof(GetDepartments)}] - Error while retrive all departments: {ex}");
                throw ex;
            }
        }
        public async Task<bool> UpdateDepartment(Department department)
        {
            try
            {
                DepartmentDTO? departmentDTO = await _context.Departments.FirstOrDefaultAsync(d => d.Name == department.Name);
                if (departmentDTO == null)
                {
                    _logger.LogError($"[{nameof(DDepartments)}] - [{nameof(UpdateDepartment)}] - Department with Name {departmentDTO?.Name} not found.");
                    return false;
                }
                else
                {
                    UserDTO? user = await _context.Users.FirstOrDefaultAsync(u => u.Email == department.ManagerEmail);
                    if (user == null || user.Role != "manager")
                    {
                        _logger.LogError($"[{nameof(DDepartments)}] - [{nameof(UpdateDepartment)}] - Employee with Name {departmentDTO?.ManagerEmail} not found or not Manager.");
                        return false;
                    }
                    else
                    {
                        ValidateDepartment(department);
                        departmentDTO.ManagerEmail = department.ManagerEmail;
                        _logger.LogInformation($"[{nameof(DDepartments)}] - [{nameof(UpdateDepartment)}] - Department with Name {department?.Name} updated.");
                        return true;
                    }
                }
            }
            catch (ValidationException ex)
            {
                _logger.LogError($"[{nameof(DDepartments)}] - [{nameof(UpdateDepartment)}] - Validation Error while update department: {ex}");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(DDepartments)}] - [{nameof(UpdateDepartment)}] - Error while update department: {ex}");
                throw ex;
            }
        }
        public async Task<List<Dictionary<string, string>>>? DepartmentsStatistics()
        {
            try
            {
                var departmentsInfo = await (from emp in _context.Employees
                                       join department in _context.Departments
                                       on emp.DepartmentName equals department.Name
                                       group emp by department.Name into DG
                                       select new Dictionary<string, string>
                                       {
                                          { "departmentName", DG.Key },
                                          { "numberOfEmployees", DG.Count().ToString() }
                                       }
                                       ).ToListAsync();
                _logger.LogInformation($"[{nameof(DDepartments)}] - [{nameof(DepartmentsStatistics)}] - Retrived departments statistics");
                return departmentsInfo;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(DDepartments)}] - [{nameof(DepartmentsStatistics)}] - Error while retrive departments statistics: {ex}");
                throw ex;
            }
        }
        private void ValidateDepartment(Department department)
        {
            var validationContext = new ValidationContext(department, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();

            if (!Validator.TryValidateObject(department, validationContext, validationResults, validateAllProperties: true))
            {
                var errorMessages = validationResults.Select(result => result.ErrorMessage);
                throw new ValidationException(string.Join(Environment.NewLine, errorMessages));
            }
        }
    }
}
