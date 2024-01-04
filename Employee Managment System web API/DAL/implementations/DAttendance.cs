using Employee_Management_System.Model;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Employee_Management_System.DAL
{
    public class DAttendance : IDAttendance
    {
        private readonly AppDbContext _context;
        private readonly ILogger<DAttendance> _logger;

        public DAttendance(AppDbContext context, ILogger<DAttendance> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<bool> AddAttendance(Attendance attendance)
        {
            try
            {
                if (attendance == null)
                {
                    Console.WriteLine("Bad Request should has a attendance to add it");
                    _logger.LogError("Bad Request should has a attendance to add it");
                    return false;
                }
                else
                {
                    ValidateAttendance(attendance);
                    AttendanceDTO attendanceDTO = new AttendanceDTO()
                    {
                        EmployeeEmail = attendance.EmployeeEmail,
                        CheckIn = attendance.CheckIn,
                        CheckOut = attendance.CheckOut,
                        Status = Enum.Parse<AttendanceStatus>(attendance.Status),
                    };
                    _context.Attendances.Add(attendanceDTO);
                    _context.SaveChanges();
                    var id = attendance.Id;
                    _logger.LogInformation($"Added new Attendance for: {attendance.EmployeeEmail}");
                    return true;
                }
            }
            catch (ValidationException ex)
            {
                _logger.LogError($"[{nameof(DAttendance)}] - [{nameof(AddAttendance)}] - Validation Error while add new attendance: {ex}");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(DAttendance)}] - [{nameof(AddAttendance)}] - Error while add new attendance: {ex}");
                throw ex;
            }
        }
        public bool UpdateAttendance(Attendance attendance)
        {
            try
            {
                AttendanceDTO? attendanceDTO = _context.Attendances.FirstOrDefault(a => a.Id == attendance.Id);
                if (attendanceDTO == null)
                {
                    Console.WriteLine($"Attendance with Id {attendance.Id} not found.");
                    _logger.LogError($"Attendance with Id {attendance.Id} not found.");
                    return false;
                }
                else
                {
                    ValidateAttendance(attendance);
                    attendanceDTO.Status = Enum.Parse<AttendanceStatus>(attendance.Status);
                    attendanceDTO.CheckIn = attendance.CheckIn;
                    attendanceDTO.CheckOut = attendance.CheckOut;
                    _context.SaveChanges();
                    _logger.LogInformation($"[{nameof(DAttendance)}] - [{nameof(UpdateAttendance)}] - Attendance with Id {attendance.Id} updated.");
                    return true;
                }
            }
            catch (ValidationException ex)
            {
                _logger.LogError($"[{nameof(DAttendance)}] - [{nameof(UpdateAttendance)}] - Validation Error while update attendance: {ex}");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(DAttendance)}] - [{nameof(UpdateAttendance)}] - Error while update attendance: {ex}");
                throw ex;
            }
        }
        public List<Attendance>? GetAttendances(string employeeEmail)
        {
            try
            {
                var attendances = _context.Attendances
                    .Where(a => a.EmployeeEmail == employeeEmail)
                    .Select(a => new Attendance
                    {
                        Id = a.Id,
                        EmployeeEmail = a.EmployeeEmail,
                        CheckOut = a.CheckOut,
                        CheckIn = a.CheckIn,
                        Status = a.Status.ToString(),
                    })
                    .ToList();

                _logger.LogInformation($"[{nameof(DAttendance)}] - [{nameof(GetAttendances)}] - Retrieved attendances for employee with email {employeeEmail}");
                return attendances;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(DAttendance)}] - [{nameof(GetAttendances)}] - Error while retrieving attendances for employee: {ex}");
                throw ex;
            }
        }
        public async Task<List<Attendance>> GetAttendances(DateTime startDate)
        {
            try
            {
                var attendances = _context.Attendances
                   .Where(a => a.CheckIn >= startDate)
                   .Select(a => new Attendance
                   {
                       Id = a.Id,
                       EmployeeEmail = a.EmployeeEmail,
                       CheckOut = a.CheckOut,
                       CheckIn = a.CheckIn,
                       Status = a.Status.ToString(),
                   })
                   .OrderBy(a => a.EmployeeEmail)
                   .ThenBy(a => a.CheckIn)
                   .ToList();
                _logger.LogInformation($"[{nameof(DAttendance)}] - [{nameof(GetAttendances)}] - retrived all attendances");
                return attendances;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(DAttendance)}] - [{nameof(GetAttendances)}] - Error while retrive all attendances: {ex}");
                throw ex;
            }
        }
        public async Task<List<Attendance>>? GetAttendanceReport(string employeeEmail, DateTime startDate)
        {
            try
            {
                var attendances = _context.Attendances
                .Where(a => a.EmployeeEmail == employeeEmail && a.CheckIn >= startDate)
                .Select(a => new Attendance
                {
                    Id = a.Id,
                    EmployeeEmail = a.EmployeeEmail,
                    CheckOut = a.CheckOut,
                    CheckIn = a.CheckIn,
                    Status = a.Status.ToString(),
                })
                .ToList();
                _logger.LogInformation($"[{nameof(DAttendance)}] - [{nameof(GetAttendanceReport)}] - Get Attendance Report");
                return attendances;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(DAttendance)}] - [{nameof(GetAttendanceReport)}] - Error while get attendance report: {ex}");
                throw ex;
            }
        }
        public async Task<List<Attendance>> GetAttendancesForDepartment(string departmentName)
        {
            try
            {
                var attendances = (from emp in _context.Employees
                                   join attendance in _context.Attendances on  emp.UserEmail equals attendance.EmployeeEmail 
                                   where emp.DepartmentName == departmentName
                                   select new Attendance
                                   {
                                       Id = attendance.Id,
                                       EmployeeEmail = attendance.EmployeeEmail,
                                       CheckOut = attendance.CheckOut,
                                       CheckIn = attendance.CheckIn,
                                       Status = attendance.Status.ToString(),
                                   }).ToList();
                return attendances;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(DAttendance)}] - [{nameof(GetAttendanceReportForDepartment)}] - Error while get attendances for department: {ex}");
                throw ex;
            }
        }
        public async Task<List<Attendance>>? GetAttendanceReportForDepartment(string departmentName, DateTime? startDate)
        {
            try
            {
                var attendances = (from emp in _context.Employees
                                   join attendance in _context.Attendances on emp.UserEmail equals attendance.EmployeeEmail
                                   where (attendance.CheckIn >= startDate && emp.DepartmentName == departmentName)
                                      select new Attendance
                                      {
                                          Id = attendance.Id,
                                          EmployeeEmail = attendance.EmployeeEmail,
                                          CheckOut = attendance.CheckOut,
                                          CheckIn = attendance.CheckIn,
                                          Status = attendance.Status.ToString(),
                                      }).ToList();
                return attendances;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(DAttendance)}] - [{nameof(GetAttendanceReportForDepartment)}] - Error while get attendance report: {ex}");
                throw ex;
            }
        }
        private void ValidateAttendance(Attendance attendance)
        {
            var validationContext = new ValidationContext(attendance, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();

            if (!Validator.TryValidateObject(attendance, validationContext, validationResults, validateAllProperties: true)|| !Enum.TryParse<AttendanceStatus>(attendance.Status, out AttendanceStatus status))
            {
                var errorMessages = validationResults.Select(result => result.ErrorMessage);
                throw new ValidationException(string.Join(Environment.NewLine, errorMessages));
            }
        }
    }
}
