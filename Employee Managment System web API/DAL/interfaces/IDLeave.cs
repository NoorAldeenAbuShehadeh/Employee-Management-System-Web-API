using Employee_Management_System.Model;
namespace Employee_Management_System.DAL
{
    public interface IDLeave
    {
        public Task<bool> AddLeave(Leave leave);
        public Task<bool> UpdateLeave(Leave leave);
        public Leave? GetLeave(Guid id);
        public Task<List<Leave>>? GetLeaves(string employeeEmail);
        public Task<List<Leave>> GetLeaves();
        public Task<List<Leave>>? GetPendingLeaves(string departmentName);
        public Task<List<Leave>>? GetLeavesForDepartment(string departmentName);
        public Task<List<Leave>> GetApprovedLeaves();
    }
}
