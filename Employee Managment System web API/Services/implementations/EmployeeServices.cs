using Employee_Management_System.DAL;
using Employee_Management_System.Model;
using Microsoft.AspNetCore.Mvc;

namespace Employee_Management_System.Services
{
    public class EmployeeServices : IEmployeeServices
    {
        private IDUsers _dUsers;
        private IDEmployees _dEmployee;
        private IDSalary _dSalary;
        private IDDepartments _dDepartments;
        private IDCommitDBChanges _dCommitDBChanges;
        private ILogger<EmployeeServices> _logger;
        public EmployeeServices(IDUsers dUsers, IDEmployees dEmployees, IDSalary dSalary, IDDepartments dDepartments,IDCommitDBChanges dCommitDBChanges, ILogger<EmployeeServices> logger)
        {
            _dUsers = dUsers;
            _dEmployee = dEmployees;
            _dSalary = dSalary;
            _dDepartments = dDepartments;
            _dCommitDBChanges = dCommitDBChanges;
            _logger = logger;
        }
        public async Task<bool> AddEmployee(EmployeeCreationModel employeeCreationModel)
        {
            try
            {
                var user = new User
                {
                    Email = employeeCreationModel.Email,
                    Name = employeeCreationModel.Name,
                    Password = employeeCreationModel.Password,
                    Role = employeeCreationModel.Role,
                    Status = "active"
                };
                var employee = new Employee
                {
                    UserEmail = employeeCreationModel.Email,
                    Address = employeeCreationModel.Address,
                    DepartmentName = employeeCreationModel.DepartmentName,
                    PhoneNumber = employeeCreationModel.PhoneNumber,
                };
                var salary = new Salary
                {
                    EmployeeEmail = employeeCreationModel.Email,
                    Amount = employeeCreationModel.SalaryAmount,
                    Bonuses = employeeCreationModel.SalaryBonuses,
                    Deductions = employeeCreationModel.SalaryDeductions
                };
                if (await _dUsers.AddUser(user) && await _dEmployee.AddEmployee(employee) && await _dSalary.AddSalary(salary))
                {
                    _dCommitDBChanges.SaveChanges();
                    if (employeeCreationModel.Role == "manager")
                    {
                        _dDepartments.UpdateDepartment(new Department { Name = employeeCreationModel.DepartmentName, ManagerEmail = employeeCreationModel.Email });
                    }
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(EmployeeServices)}] - [{nameof(AddEmployee)}] - Error while add new employee: {ex}");
                throw ex;
            }
        }
        public async Task<List<Employee>> GetEmployees()
        {
            try
            {
                return await _dEmployee.GetEmployees();
            }
            catch(Exception ex)
            {
                _logger.LogError($"[{nameof(EmployeeServices)}] - [{nameof(GetEmployees)}] - Error while get all employees: {ex}");
                throw ex;
            }
        }
        public async Task<(List<Employee>, bool)> GetEmployeesInDepartment(User user, string departmentName)
        {
            try
            {
                if (user.Role == "manager")
                {
                    Employee employee = await _dEmployee.GetEmployee(user.Email);
                    if (employee == null || employee?.DepartmentName != departmentName)
                    {
                        return (null, false);
                    }
                }
                var result = await _dEmployee.GetEmployees(departmentName);
                return (result, true);
            }
            catch(Exception ex)
            {
                _logger.LogError($"[{nameof(EmployeeServices)}] - [{nameof(GetEmployeesInDepartment)}] - Error while get employees in department: {ex}");
                throw ex;
            }
        }

    }
}
