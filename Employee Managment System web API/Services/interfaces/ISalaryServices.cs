using Employee_Management_System.Model;

namespace Employee_Management_System.Services
{
    public interface ISalaryServices
    {
        public Task<List<Dictionary<string, string>>> GetEmployeeSalaries();
        public Task<Salary> GetEmployeeSalaryDetails(User user, string? employeeEmail);
        public Task<bool> UpdateSalary(Salary salary);
    }
}
