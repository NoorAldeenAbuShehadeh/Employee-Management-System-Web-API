using Employee_Management_System.DAL;
using Employee_Management_System.Model;
using log4net.Core;
using Microsoft.AspNetCore.Mvc;

namespace Employee_Management_System.Services
{
    public class LeaveServices : ILeaveServices
    {
        private IDLeave _dLeave;
        private IDEmployees _dEmployee;
        private ILogger<LeaveServices> _logger;
        public LeaveServices(IDLeave dLeave, IDEmployees dEmployees, ILogger<LeaveServices> logger)
        {
            _dLeave = dLeave;
            _dEmployee = dEmployees;
            _logger = logger;
        }
        public async Task<bool> AddLeave(Leave leave)
        {
            try
            {
                bool leaveAdded = await _dLeave.AddLeave(leave);
                if (leaveAdded)
                    return true;
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(LeaveServices)}] - [{nameof(AddLeave)}] - Error while add new leave: {ex}");
                throw ex;
            }
        }
        public async Task<List<Leave>> GetLeaves()
        {
            try
            {
                var result = await _dLeave.GetLeaves();
                return result;
            }
            catch(Exception ex)
            {
                _logger.LogError($"[{nameof(LeaveServices)}] - [{nameof(GetLeaves)}] - Error while get all leaves: {ex}");
                throw ex;
            }
        }
        public async Task<List<Dictionary<string, string>>> LeaveTrend()
        {
            try
            {
                var approvedLeaves = await _dLeave.GetApprovedLeaves();
                var result = approvedLeaves?.GroupBy(l => l.EmployeeEmail)?
                         .OrderByDescending(lg => lg.Count())?
                         .Select(lg => new Dictionary<string, string>
                         {
                            { "employeeEmail", lg.Key },
                            { "totalLeaves", lg.Count().ToString() }
                         })?
                         .ToList();
                return result;
            }
            catch(Exception ex)
            {
                _logger.LogError($"[{nameof(LeaveServices)}] - [{nameof(LeaveTrend)}] - Error while get teave trend: {ex}");
                throw ex;
            }
        }
        public async Task<(List<Leave>, bool)> GetDepartmentLeaves(User user, string departmentName)
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
                var leaves = await _dLeave.GetLeavesForDepartment(departmentName);
                return (leaves, true);
            }
            catch(Exception ex)
            {
                _logger.LogError($"[{nameof(LeaveServices)}] - [{nameof(GetDepartmentLeaves)}] - Error while get department leaves: {ex}");
                throw ex;
            }
        }
        public async Task<(List<Leave>, bool)> GetPendingLeavesForDepartment(User user, string departmentName)
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
                var leaves = await _dLeave.GetPendingLeaves(departmentName);
                return (leaves, true);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(LeaveServices)}] - [{nameof(GetPendingLeavesForDepartment)}] - Error while get pending leaves for department: {ex}");
                throw ex;
            }
        }
        public async Task<(List<Leave>, bool)> GetLeavesForEmployee(User user, string? employeeEmail)
        {
            try
            {
                if (user.Role == "employee")
                {
                    var leaves = await _dLeave.GetLeaves(user.Email);
                    return (leaves, true);
                }
                else if (employeeEmail != null)
                {
                    if (user.Role == "manager")
                    {
                        Employee? employee = await _dEmployee.GetEmployee(employeeEmail);
                        Employee? manager = await _dEmployee.GetEmployee(user.Email);
                        if (manager.DepartmentName == employee.DepartmentName)
                        {
                            var leaves = await _dLeave.GetLeaves(employeeEmail);
                            return (leaves, true);
                        }
                    }
                    else if (user.Role == "admin")
                    {
                        var leaves = await _dLeave.GetLeaves(employeeEmail);
                        return (leaves, true);
                    }
                }
                return (null, false);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(LeaveServices)}] - [{nameof(GetLeavesForEmployee)}] - Error while retrieving leave report for an employee: {ex}");
                throw ex;
            }
        }

    }
}
