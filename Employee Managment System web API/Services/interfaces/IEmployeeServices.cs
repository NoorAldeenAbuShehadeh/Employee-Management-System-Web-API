using Employee_Management_System.Model;
using Microsoft.AspNetCore.Mvc;

namespace Employee_Management_System.Services
{
    public interface IEmployeeServices
    {
        public Task<bool> AddEmployee(EmployeeCreationModel employeeCreationModel);
        public Task<List<Employee>> GetEmployees();
        public Task<(List<Employee>, bool)> GetEmployeesInDepartment(User user, string departmentName);
        public Task<List<Employee>> FilterEmployeesBySalary(decimal minSalary);
        public Task<List<Employee>> SearchForEmployeesByCity(string city);
        public Task<bool> UpdateEmployee(UpdateEmployeeInfo updateEmployeeInfo);
        public Task<(Employee, User)> GetEmployee(string employeeEmail);
        public Task<bool> UpdateGeneralEmployeeInfo(Employee employee);
    }
}
