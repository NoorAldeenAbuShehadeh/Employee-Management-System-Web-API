using Employee_Management_System.Model;
using Microsoft.AspNetCore.Mvc;

namespace Employee_Management_System.Services
{
    public interface IEmployeeServices
    {
        public Task<bool> AddEmployee(EmployeeCreationModel employeeCreationModel);
        public Task<List<Employee>> GetEmployees();
        public Task<(List<Employee>, bool)> GetEmployeesInDepartment(User user, string departmentName);
    }
}
