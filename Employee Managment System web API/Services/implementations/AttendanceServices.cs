using Employee_Management_System.DAL;
using Employee_Management_System.Model;

namespace Employee_Management_System.Services
{
    public class AttendanceServices : IAttendanceServices
    {
        private IDAttendance _dAttendance;
        private IDEmployees _dEmployee;
        private ILogger<AttendanceServices> _logger;
        public AttendanceServices(IDAttendance dAttendance, IDEmployees dEmployee, ILogger<AttendanceServices> logger)
        {
            _dAttendance = dAttendance;
            _dEmployee = dEmployee;
            _logger = logger;
        }
        public async Task<bool> AddAttendance(Attendance attendance, User user)
        {
            try
            {
                var att = new Attendance
                {
                    EmployeeEmail = user.Email,
                    CheckIn = attendance.CheckIn,
                    CheckOut = attendance.CheckOut,
                    Status = attendance.Status,
                };
                bool added = await _dAttendance.AddAttendance(att);
                return added;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(AttendanceServices)}] - [{nameof(AddAttendance)}] - Error while add new attendance: {ex}");
                throw ex;
            }
        }
        public async Task<(List<Attendance>, bool)> GetAttendancesForDepartment(string departmentName, DateTime? startDate, User user)
        {
            try
            {
                if (user.Role == "manager")
                {
                    var employee = await _dEmployee.GetEmployee(user.Email);
                    if (employee == null || employee?.DepartmentName != departmentName)
                    {
                        return (null, false);
                    }
                }
                if (startDate != null)
                {
                    var attendances = await _dAttendance.GetAttendanceReportForDepartment(departmentName, startDate);
                    return (attendances, true);
                }
                var result = await _dAttendance.GetAttendancesForDepartment(departmentName);
                return (result, true);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(AttendanceServices)}] - [{nameof(GetAttendancesForDepartment)}] - Error while get attendances for department: {ex}");
                throw ex;
            }
        }
        public async Task<List<Attendance>> AttendancesReport(DateTime startDate)
        {
            try
            {
                var attendances = await _dAttendance.GetAttendances(startDate);
                return attendances;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(AttendanceServices)}] - [{nameof(AttendancesReport)}] - Error while retrieving attendance report: {ex}");
                throw ex;
            }
        }
        public async Task<(List<Attendance>, bool)> GetAttendanceForEmployee(User user, string? employeeEmail, DateTime startDate)
        {
            try
            {
                if (user.Role == "employee")
                {
                    var attendances = _dAttendance.GetAttendanceReport(user.Email, startDate);
                    return (attendances, true);
                }
                else if (employeeEmail != null)
                {
                    if (user.Role == "manager")
                    {
                        Employee? employee = await _dEmployee.GetEmployee(employeeEmail);
                        Employee? manager = await _dEmployee.GetEmployee(user.Email);
                        if (manager.DepartmentName == employee.DepartmentName)
                        {
                            var attendances = _dAttendance.GetAttendanceReport(employeeEmail, startDate);
                            return (attendances, true);
                        }
                    }
                    else if (user.Role == "admin")
                    {
                        var attendances = _dAttendance.GetAttendanceReport(employeeEmail, startDate);
                        return(attendances, true);
                    }
                }
                return (null, false);
            }
            catch(Exception ex)
            {
                _logger.LogError($"[{nameof(AttendanceServices)}] - [{nameof(GetAttendanceForEmployee)}] - Error while retrieving attendance report for an employee: {ex}");
                throw ex;
            }
        }
    }
}
