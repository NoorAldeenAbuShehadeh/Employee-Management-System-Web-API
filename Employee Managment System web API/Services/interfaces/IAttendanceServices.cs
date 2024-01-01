using Employee_Management_System.Model;
using Microsoft.AspNetCore.Mvc;

namespace Employee_Management_System.Services
{
    public interface IAttendanceServices
    {
        public Task<bool> AddAttendance(Attendance attendance, User user);
        public Task<(List<Attendance>, bool)> GetAttendancesForDepartment(string departmentName, DateTime? startDate, User user);
        public Task<List<Attendance>> AttendancesReport(DateTime startDate);
        public Task<(List<Attendance>, bool)> GetAttendanceForEmployee(User user, string? employeeEmail, DateTime startDate);
    }
}
