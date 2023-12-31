using Employee_Management_System.Model;
using Microsoft.AspNetCore.Mvc;

namespace Employee_Management_System.DAL
{
    public interface IDAttendance
    {
        public Task<bool> AddAttendance(Attendance attendance);
        public bool UpdateAttendance(Attendance attendance);
        public List<Attendance>? GetAttendances(string employeeEmail);
        public Task<List<Attendance>> GetAttendancesForDepartment(string departmentName);
        public Task<List<Attendance>> GetAttendances(DateTime startDate);
        public List<Attendance>? GetAttendanceReport(string employeeEmail, DateTime startDate);
        public List<Attendance>? GetAttendanceReportForDepartment(string departmentName, DateTime startDate);
    }
}
