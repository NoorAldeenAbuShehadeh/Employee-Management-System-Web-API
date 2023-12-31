﻿using Employee_Management_System.Model;
using System.ComponentModel.DataAnnotations;

namespace Employee_Management_System.DAL
{
    internal class DEmployees : IDEmployees
    {
        private readonly AppDbContext _context;
        private readonly ILogger<DEmployees> _logger;
        public DEmployees(AppDbContext context, ILogger<DEmployees> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<bool> AddEmployee(Employee employee)
        {
            try
            {
                if (employee == null)
                {
                    Console.WriteLine("Bad Request should has a employee to add it");
                    _logger.LogError("Bad Request should has a employee to add it");
                    return false;
                }
                else
                {
                    ValidateEmployee(employee);
                    EmployeeDTO employeeDTO = new EmployeeDTO()
                    {
                        UserEmail = employee.UserEmail,
                        Address = employee.Address,
                        DepartmentName = employee.DepartmentName,
                        PhoneNumber = employee.PhoneNumber
                    };
                    _context.Employees.Add(employeeDTO);
                    _logger.LogInformation($"[{nameof(DEmployees)}] - [{nameof(AddEmployee)}] - Added new Employee: {employee.UserEmail}");
                    return true;
                }
            }
            catch (ValidationException ex)
            {
                _logger.LogError($"[{nameof(DEmployees)}] - [{nameof(AddEmployee)}] - Validation Error while add new employee: {ex}");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(DEmployees)}] - [{nameof(AddEmployee)}] - Error while add new employee: {ex}");
                throw ex;
            }
        }
        public bool UpdateEmployee(Employee employee)
        {
            try
            {
                EmployeeDTO? employeeDTO = _context.Employees.FirstOrDefault(e => e.UserEmail == employee.UserEmail);
                if (employeeDTO == null)
                {
                    Console.WriteLine($"Employee with email {employee?.UserEmail} not found.");
                    _logger.LogError($"Employee with email {employee?.UserEmail} not found.");
                    return false;
                }
                else
                {
                    ValidateEmployee(employee);
                    employeeDTO.PhoneNumber = employee.PhoneNumber;
                    employeeDTO.Address = employee.Address;
                    employeeDTO.DepartmentName = employee.DepartmentName;
                    _logger.LogInformation($"[{nameof(DEmployees)}] - [{nameof(UpdateEmployee)}] - Employee with email {employee.UserEmail} updated.");
                    return true;
                }
            }
            catch (ValidationException ex)
            {
                _logger.LogError($"[{nameof(DEmployees)}] - [{nameof(UpdateEmployee)}] - Validation Error while update employee: {ex}");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(DEmployees)}] - [{nameof(UpdateEmployee)}] - Error while update employee: {ex}");
                throw ex;
            }
        }
        public async Task<List<Employee>> GetEmployees()
        {
            try
            {
                var employees = _context.Employees
                    .Select(e => new Employee
                    {
                        UserEmail = e.UserEmail,
                        Address = e.Address,
                        DepartmentName = e.DepartmentName,
                        PhoneNumber = e.PhoneNumber
                    })
                    .ToList();
                _logger.LogInformation($"[{nameof(DEmployees)}] - [{nameof(GetEmployees)}] - Retrived all employees");
                return employees;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(DEmployees)}] - [{nameof(GetEmployees)}] - Error while retrive all employees: {ex.Message}");
                throw ex;
            }
        }
        public async Task<List<Employee>> GetEmployees(string departmentName)
        {
            try
            {
                var employeeDTOs = (from emp in _context.Employees
                                    join user in _context.Users on emp.DepartmentName equals departmentName
                                    where (emp.UserEmail == user.Email && user.Role == "employee")
                                    select new Employee
                                    {
                                        UserEmail = emp.UserEmail,
                                        Address = emp.Address,
                                        DepartmentName = emp.DepartmentName,
                                        PhoneNumber = emp.PhoneNumber,
                                    }).ToList();
                _logger.LogInformation($"[{nameof(DEmployees)}] - [{nameof(GetEmployees)}] - Retrived all employees in department");
                return employeeDTOs;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(DEmployees)}] - [{nameof(GetEmployees)}] - Error while retrive employees in a department: {ex}");
                throw ex;
            }
        }
        public async Task<Employee> GetEmployee(string email)
        {
            try
            {
                EmployeeDTO? employeeDTO = _context.Employees.FirstOrDefault(e => e.UserEmail == email);
                if (employeeDTO == null)
                {
                    Console.WriteLine($"Employee with email {email} not found.");
                    _logger.LogError($"Employee with email {email} not found.");
                    return null;
                }
                Employee employee = new Employee
                {
                    UserEmail = employeeDTO.UserEmail,
                    Address = employeeDTO.Address,
                    DepartmentName = employeeDTO.DepartmentName,
                    PhoneNumber = employeeDTO.PhoneNumber
                };
                _logger.LogInformation($"[{nameof(DEmployees)}] - [{nameof(GetEmployee)}] - Get data for employee");
                return employee;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(DEmployees)}] - [{nameof(GetEmployee)}] - Error while get employee: {ex.Message}");
                throw ex;
            }
        }
        public List<Employee>? GetEmployees(decimal minSalary)
        {
            try
            {
                var employees = (from emp in _context.Employees
                                 join user in _context.Users on emp.UserEmail equals user.Email
                                 join salary in _context.Salaries on emp.UserEmail equals salary.EmployeeEmail
                                 where (salary.Amount >= minSalary)
                                 select new Employee
                                 {
                                     UserEmail = emp.UserEmail,
                                     Address = emp.Address,
                                     DepartmentName = emp.DepartmentName,
                                     PhoneNumber = emp.PhoneNumber,
                                 }).ToList();
                _logger.LogInformation($"[{nameof(DEmployees)}] - [{nameof(GetEmployees)}] - Retrived all employees have min salary = {minSalary}");
                return employees;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(DEmployees)}] - [{nameof(GetEmployees)}] - Error while retrive all employees have min salary = {minSalary}: {ex}");
                throw ex;
            }
        }
        public List<Employee>? SerchEmployeesbyCityName(string cityName)
        {
            try
            {
                var employees = _context.Employees
                    .Where(emp => emp.Address.ToLower().Contains(cityName.Trim().ToLower()))
                    .Select(emp => new Employee
                    {
                        UserEmail = emp.UserEmail,
                        Address = emp.Address,
                        DepartmentName = emp.DepartmentName,
                        PhoneNumber = emp.PhoneNumber,
                    }).ToList();

                _logger.LogInformation($"[{nameof(DEmployees)}] - [{nameof(SerchEmployeesbyCityName)}] - Retrived all employees lives in specific city");
                return employees;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(DEmployees)}] - [{nameof(SerchEmployeesbyCityName)}] - Error while retrive employees lives in specific city: {ex}");
                throw ex;
            }
        }
        private void ValidateEmployee(Employee employee)
        {
            var validationContext = new ValidationContext(employee, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();

            if (!Validator.TryValidateObject(employee, validationContext, validationResults, validateAllProperties: true))
            {
                var errorMessages = validationResults.Select(result => result.ErrorMessage);
                throw new ValidationException(string.Join(Environment.NewLine, errorMessages));
            }
        }
    }
}
