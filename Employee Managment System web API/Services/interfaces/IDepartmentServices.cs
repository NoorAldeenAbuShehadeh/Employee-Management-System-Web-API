using Employee_Management_System.Model;

namespace Employee_Management_System.Services
{
    public interface IDepartmentServices
    {
        public Task<bool> AddDepartment(Department department);
        public Task<List<Department>> GetDepartments();
        public Task<List<Dictionary<string, string>>> GetDepartmentSatatistics();
        public Task<bool> UpdateDepartment(Department department);
    }
}
