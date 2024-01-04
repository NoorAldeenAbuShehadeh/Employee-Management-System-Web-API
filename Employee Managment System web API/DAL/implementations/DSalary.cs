using Employee_Management_System.Model;
using System.ComponentModel.DataAnnotations;

namespace Employee_Management_System.DAL
{
    public class DSalary : IDSalary
    {
        private readonly AppDbContext _context;
        private readonly ILogger<DSalary> _logger;

        public DSalary(AppDbContext context, ILogger<DSalary> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<bool> AddSalary(Salary salary)
        {
            try
            {
                if (salary == null)
                {
                    Console.WriteLine("Bad Request should has a salary to add it");
                    _logger.LogError("Bad Request should has a salary to add it");
                    return false;
                }
                else
                {
                    ValidateSalary(salary);
                    SalaryDTO salaryDTO = new SalaryDTO()
                    {
                        EmployeeEmail = salary.EmployeeEmail,
                        Amount = salary.Amount,
                        Bonuses = salary.Bonuses,
                        Deductions = salary.Deductions,
                    };
                    _context.Salaries.Add(salaryDTO);
                    _logger.LogInformation($"[{nameof(DSalary)}] - [{nameof(AddSalary)}] - Added salary for employee: {salary.EmployeeEmail}");
                    return true;
                }
            }
            catch (ValidationException ex)
            {
                _logger.LogError($"[{nameof(DSalary)}] - [{nameof(AddSalary)}] - Validation Error while add new salary: {ex}");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(DSalary)}] - [{nameof(AddSalary)}] - Error while add salary: {ex}");
                throw ex;
            }
        }
        public async Task<bool> UpdateSalary(Salary salary)
        {
            try
            {
                SalaryDTO? salaryDTO = _context.Salaries.FirstOrDefault(s => s.EmployeeEmail == salary.EmployeeEmail);
                if (salaryDTO == null)
                {
                    Console.WriteLine($"salary with employee email {salary?.EmployeeEmail} not found.");
                    _logger.LogError($"salary with employee email {salary?.EmployeeEmail} not found.");
                    return false;
                }
                else
                {
                    ValidateSalary(salary);
                    salaryDTO.Amount = salary.Amount;
                    salaryDTO.Bonuses = salary.Bonuses;
                    salaryDTO.Deductions = salary.Deductions;
                    _context.SaveChanges();
                    _logger.LogInformation($"[{nameof(DSalary)}] - [{nameof(UpdateSalary)}] - salary with employee email {salary?.EmployeeEmail} updated.");
                    return true;
                }
            }
            catch (ValidationException ex)
            {
                _logger.LogError($"[{nameof(DSalary)}] - [{nameof(UpdateSalary)}] - Validation Error while update salary: {ex}");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(DSalary)}] - [{nameof(UpdateSalary)}] - Error while update salary: {ex}");
                throw ex;
            }
        }
        public async Task<Salary> GetSalary(string employeeEmail)
        {
            try
            {
                SalaryDTO? salaryDTO = _context.Salaries.FirstOrDefault(s => s.EmployeeEmail == employeeEmail.Trim());
                if (salaryDTO == null)
                {
                    Console.WriteLine($"salary with employee email {employeeEmail} not found.");
                    _logger.LogError($"salary with employee email {employeeEmail} not found.");
                    return null;
                }
                else
                {
                    Salary salary = new Salary()
                    {
                        EmployeeEmail = salaryDTO.EmployeeEmail,
                        Amount = salaryDTO.Amount,
                        Bonuses = salaryDTO.Bonuses,
                        Deductions = salaryDTO.Deductions
                    };
                    _logger.LogInformation($"[{nameof(DSalary)}] - [{nameof(GetSalary)}] - Salary with employee email {employeeEmail} returned.");
                    return salary;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(DSalary)}] - [{nameof(GetSalary)}] - Error while find salary: {ex}");
                throw ex;
            }
        }
        public async Task<List<Salary>> GetSalaries()
        {
            try
            {
                var salarys = _context.Salaries
                    .Select(s => new Salary
                    {
                        Amount = s.Amount,
                        Bonuses = s.Bonuses,
                        Deductions = s.Deductions,
                        EmployeeEmail = s.EmployeeEmail
                    })
                    .ToList();
                _logger.LogInformation($"[{nameof(DSalary)}] - [{nameof(GetSalaries)}] - Retrived all salaries");
                return salarys;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(DSalary)}] - [{nameof(GetSalaries)}] - Error while retrive all salaries: {ex}");
                throw ex;
            }
        }
        private void ValidateSalary(Salary salary)
        {
            var validationContext = new ValidationContext(salary, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();

            if (!Validator.TryValidateObject(salary, validationContext, validationResults, validateAllProperties: true))
            {
                var errorMessages = validationResults.Select(result => result.ErrorMessage);
                throw new ValidationException(string.Join(Environment.NewLine, errorMessages));
            }
        }
    }
}
