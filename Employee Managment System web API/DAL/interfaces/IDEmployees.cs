using Employee_Management_System.Model;

namespace Employee_Management_System.DAL
{
    public interface IDEmployees
    {
        public Task<bool> AddEmployee(Employee employee);
        public bool UpdateEmployee(Employee employee);
        public Task<List<Employee>> GetEmployees();
        public Task<Employee> GetEmployee(string email);
        public Task<List<Employee>> GetEmployees(string departmentName);
        public List<Employee>? GetEmployees(decimal minSalary);
        public List<Employee>? SerchEmployeesbyCityName(string cityName);
    }
}
