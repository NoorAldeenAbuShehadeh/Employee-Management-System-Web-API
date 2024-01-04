using Employee_Management_System.Model;

namespace Employee_Management_System.DAL
{
    public interface IDDepartments
    {
        public Task<bool> AddDepartment(Department department);
        public Task<bool> UpdateDepartment(Department department);
        public Task<List<Department>> GetDepartments();
        public Task<List<Dictionary<string, string>>>? DepartmentsStatistics();
    }
}
