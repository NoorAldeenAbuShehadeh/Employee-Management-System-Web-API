using Employee_Management_System.DAL;
using Employee_Management_System.Model;
using Microsoft.AspNetCore.Mvc;

namespace Employee_Management_System.Services
{
    public class DepartmentServices : IDepartmentServices
    {
        private IDDepartments _dDepartments;
        private IDEmployees _dEmployee;
        private IDCommitDBChanges _dCommitDBChanges;
        private ILogger<DepartmentServices> _logger;
        public DepartmentServices(IDDepartments dDepartments, IDEmployees dEmployees, IDCommitDBChanges dCommitDBChanges, ILogger<DepartmentServices> logger)
        {
            _dDepartments = dDepartments;
            _dEmployee = dEmployees;
            _dCommitDBChanges = dCommitDBChanges;
            _logger = logger;
        }
        public async Task<bool> AddDepartment(Department department)
        {
            try
            {
                bool departmentAdded = await _dDepartments.AddDepartment(new Department { Name = department.Name, ManagerEmail = null });
                if (departmentAdded)
                {
                    return true;
                }
                return false;
            }
            catch(Exception ex)
            {
                _logger.LogError($"[{nameof(DepartmentServices)}] - [{nameof(AddDepartment)}] - Error while add new department: {ex}");
                throw ex;
            }
        }
        public async Task<List<Department>> GetDepartments()
        {
            try
            {
                var result = await _dDepartments.GetDepartments();
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(DepartmentServices)}] - [{nameof(GetDepartments)}] - Error while get all departments: {ex}");
                throw ex;
            }

        }
        public async Task<List<Dictionary<string, string>>> GetDepartmentSatatistics()
        {
            try
            {
                var departmentStatistics = await _dDepartments.DepartmentsStatistics();
                return departmentStatistics;
            }
            catch(Exception ex )
            {
                _logger.LogError($"[{nameof(DepartmentServices)}] - [{nameof(GetDepartmentSatatistics)}] - Error while get department statistics: {ex}");
                throw ex;
            }
        }
        public async Task<bool> UpdateDepartment(Department department)
        {
            try
            {
                bool departmentUpdated = await _dDepartments.UpdateDepartment(department);
                if (departmentUpdated)
                {
                    Employee emp = await _dEmployee.GetEmployee(department.ManagerEmail);
                    if (emp != null)
                    {
                        emp.DepartmentName = department.Name;
                        bool employeeUpdated = await _dEmployee.UpdateEmployee(emp);
                        if (employeeUpdated) { _dCommitDBChanges.SaveChanges(); }
                        return employeeUpdated;
                    }
                }
                return false;
            }
            catch(Exception ex )
            {
                _logger.LogError($"[{nameof(DepartmentServices)}] - [{nameof(UpdateDepartment)}] - Error while update department: {ex}");
                throw ex;
            }
        }

    }
}
