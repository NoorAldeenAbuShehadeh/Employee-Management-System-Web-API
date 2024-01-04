using Employee_Management_System.Model;

namespace Employee_Management_System.DAL
{
    public interface IDSalary
    {
        public Task<bool> AddSalary(Salary salary);
        public Task<bool> UpdateSalary(Salary salary);
        public Task<Salary> GetSalary(string employeeEmail);
        public Task<List<Salary>> GetSalaries();
    }
}
