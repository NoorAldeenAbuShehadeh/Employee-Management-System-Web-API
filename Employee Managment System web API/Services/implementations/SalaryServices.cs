using Employee_Management_System.DAL;
using Employee_Management_System.Model;
using Microsoft.AspNetCore.Mvc;

namespace Employee_Management_System.Services
{
    public class SalaryServices : ISalaryServices
    {
        private IDSalary _dsalary;
        private IDEmployees _dEmployees;
        private ILogger<SalaryServices> _logger;
        public SalaryServices(IDSalary dSalary, IDEmployees dEmployees, ILogger<SalaryServices> logger)
        {
            _dsalary = dSalary;
            _dEmployees = dEmployees;
            _logger = logger;
        }
        public async Task<List<Dictionary<string, string>>> GetEmployeeSalaries()
        {
            try
            {
                var salaries = await _dsalary.GetSalaries();
                var result = salaries?.Select(salary => new Dictionary<string, string>
                                  {
                                     {"Email", salary.EmployeeEmail },
                                     { "Total Salary", (salary.Amount - salary.Deductions + salary.Bonuses).ToString() }
                                   }).ToList();
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(SalaryServices)}] - [{nameof(GetEmployeeSalaries)}] - Error while get employee salaries: {ex}");
                throw ex;
            }
        }
        public async Task<Salary> GetEmployeeSalaryDetails(string employeeEmail)
        {
            try
            {
                var result = await _dsalary.GetSalary(employeeEmail);
                return result;
            }
            catch(Exception ex)
            {
                _logger.LogError($"[{nameof(SalaryServices)}] - [{nameof(GetEmployeeSalaryDetails)}] - Error while get salary details:for employee {ex}");
                throw ex;
            }
        }
        public async Task<List<Employee>> FilterEmployeesBySalary()
        {
            try
            {
                return await _dEmployees.FilterEmployeesBySalary(1000);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(SalaryServices)}] - [{nameof(FilterEmployeesBySalary)}] - Error while get employees that have salary > 1000: {ex}");
                throw ex;
            }
        }
    }
}
