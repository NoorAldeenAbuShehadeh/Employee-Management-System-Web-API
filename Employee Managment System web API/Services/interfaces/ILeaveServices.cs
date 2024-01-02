using Employee_Management_System.Model;

namespace Employee_Management_System.Services
{
    public interface ILeaveServices
    {
        public Task<bool> AddLeave(Leave leave);
        public Task<List<Leave>> GetLeaves();
        public Task<List<Dictionary<string, string>>> LeaveTrend();
        public Task<(List<Leave>, bool)> GetDepartmentLeaves(User user, string departmentName);
        public Task<(List<Leave>, bool)> GetPendingLeavesForDepartment(User user, string departmentName);
        public Task<(List<Leave>, bool)> GetLeavesForEmployee(User user, string? employeeEmail);
    }
}
