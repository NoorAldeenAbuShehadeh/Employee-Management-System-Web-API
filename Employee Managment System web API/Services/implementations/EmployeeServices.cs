using Employee_Management_System.DAL;
using Employee_Management_System.Model;

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
        public EmployeeServices(IDUsers dUsers, IDEmployees dEmployees, IDSalary dSalary, IDDepartments dDepartments, IDCommitDBChanges dCommitDBChanges, ILogger<EmployeeServices> logger)
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
                    Password = _dUsers.EncodePassword(employeeCreationModel.Password),
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
                        await _dDepartments.UpdateDepartment(new Department { Name = employeeCreationModel.DepartmentName, ManagerEmail = employeeCreationModel.Email });
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
            catch (Exception ex)
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
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(EmployeeServices)}] - [{nameof(GetEmployeesInDepartment)}] - Error while get employees in department: {ex}");
                throw ex;
            }
        }
        public async Task<List<Employee>> FilterEmployeesBySalary(decimal minSalary)
        {
            try
            {
                return await _dEmployee.FilterEmployeesBySalary(minSalary);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(EmployeeServices)}] - [{nameof(FilterEmployeesBySalary)}] - Error while get employees that have salary > 1000: {ex}");
                throw ex;
            }
        }
        public async Task<List<Employee>> SearchForEmployeesByCity(string city)
        {
            try
            {
                var employees = await _dEmployee.SearchEmployeesbyCityName(city);
                return employees;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(EmployeeServices)}] - [{nameof(SearchForEmployeesByCity)}] - Error while search for employees by city: {ex}");
                throw ex;
            }
        }
        public async Task<bool> UpdateEmployee(UpdateEmployeeInfo updateEmployeeInfo)
        {
            try
            {
                bool userUpdated = true, employeeUpdated = true;
                if (updateEmployeeInfo.Role != null)
                {
                    userUpdated = false;
                    var user = await _dUsers.GetUser(updateEmployeeInfo.EmployeeEmail);
                    if (user != null)
                    {
                        user.Role = updateEmployeeInfo.Role;
                        userUpdated = await _dUsers.UpdateUser(user);
                        if (userUpdated) _dCommitDBChanges.SaveChanges();
                    }
                }
                if (updateEmployeeInfo.DepartmentName != null)
                {
                    employeeUpdated = false;
                    var employee = await _dEmployee.GetEmployee(updateEmployeeInfo.EmployeeEmail);
                    if (employee != null)
                    {
                        employee.DepartmentName = updateEmployeeInfo.DepartmentName;
                        employeeUpdated = await _dEmployee.UpdateEmployee(employee);
                        if (userUpdated && userUpdated && updateEmployeeInfo.Role == "manager")
                        {
                            if (await _dDepartments.UpdateDepartment(new Department { ManagerEmail = employee.UserEmail, Name = updateEmployeeInfo.DepartmentName }))
                            {
                                _dCommitDBChanges.SaveChanges();
                                return true;
                            }
                            return false;
                        }
                        _dCommitDBChanges.SaveChanges();
                    }
                }
                if (userUpdated && employeeUpdated) return true;
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(EmployeeServices)}] - [{nameof(UpdateEmployee)}] - Error while update employee: {ex}");
                throw ex;
            }
        }
        public async Task<(Employee, User)> GetEmployee(string employeeEmail)
        {
            try
            {
                Employee e = await _dEmployee.GetEmployee(employeeEmail);
                User u = await _dUsers.GetUser(employeeEmail);
                return (e, u);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(EmployeeServices)}] - [{nameof(GetEmployee)}] - Error while get employee info: {ex}");
                throw ex;
            }
        }
        public async Task<bool> UpdateGeneralEmployeeInfo(Employee employee)
        {
            try
            {
                Employee e = await _dEmployee.GetEmployee(employee.UserEmail);
                if (e != null)
                {
                    if (employee.Address != null)
                    {
                        e.Address = employee.Address;
                    }
                    if (employee.PhoneNumber != null)
                    {
                        e.PhoneNumber = employee.PhoneNumber;
                    }
                    bool employeeUpdated = await _dEmployee.UpdateEmployee(e);
                    _dCommitDBChanges.SaveChanges();
                    return employeeUpdated;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(EmployeeServices)}] - [{nameof(UpdateGeneralEmployeeInfo)}] - Error while update general employee info: {ex}");
                throw ex;
            }
        }
    }
}
