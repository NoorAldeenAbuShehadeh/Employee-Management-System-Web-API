using Employee_Management_System.Model;
namespace Employee_Management_System.DAL
{
    public interface IDLeave
    {
        public Task<bool> AddLeave(Leave leave);
        public bool UpdateLeave(Leave leave);
        public Leave? GetLeave(Guid id);
        public List<Leave>? GetLeaves(string employeeEmail);
        public Task<List<Leave>> GetLeaves();
        public List<Leave>? GetPendingLeaves(string departmentName);
        public List<Leave>? GetLeavesForDepartment(string departmentName);
        public Task<List<Leave>> GetApprovedLeaves();
    }
}
