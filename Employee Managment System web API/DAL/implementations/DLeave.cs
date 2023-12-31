using Employee_Management_System.Model;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace Employee_Management_System.DAL
{
    public class DLeave : IDLeave
    {
        private readonly AppDbContext _context;
        private readonly ILogger<DLeave> _logger;

        public DLeave(AppDbContext context, ILogger<DLeave> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<bool> AddLeave(Leave leave)
        {
            try
            {
                if (leave == null)
                {
                    Console.WriteLine("Bad Request should has a leave to add it");
                    _logger.LogError("Bad Request should has a leave to add it");
                    return false;
                }
                else
                {
                    ValidateLeave(leave);
                    LeaveDTO leaveDTO = new LeaveDTO()
                    {
                        Description = leave.Description,
                        EmployeeEmail = leave.EmployeeEmail,
                        StartDate = leave.StartDate,
                        EndDate = leave.EndDate,
                        Status = Enum.Parse<LeaveStatus>(leave.Status),
                    };
                    _context.Leaves.Add(leaveDTO);
                    _context.SaveChanges();
                    _logger.LogInformation($"[{nameof(DLeave)}] - [{nameof(AddLeave)}] - Added new Leave to employee: {leaveDTO.EmployeeEmail}");
                    return true;
                }
            }
            catch (ValidationException ex)
            {
                _logger.LogError($"[{nameof(DLeave)}] - [{nameof(AddLeave)}] - Validation Error while add new leave: {ex}");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(DLeave)}] - [{nameof(AddLeave)}] - Error while add new leave: {ex}");
                throw ex;
            }
        }
        public bool UpdateLeave(Leave leave)
        {
            try
            {
                LeaveDTO? leaveDTO = _context.Leaves.FirstOrDefault(l => l.Id == leave.Id);
                if (leaveDTO == null)
                {
                    Console.WriteLine($"leave with Id {leave?.Id} not found.");
                    _logger.LogError($"leave with Id {leave?.Id} not found.");
                    return false;
                }
                else
                {
                    ValidateLeave(leave);
                    leaveDTO.StartDate = leave.StartDate;
                    leaveDTO.EndDate = leave.EndDate;
                    leaveDTO.Description = leave.Description;
                    leaveDTO.Status = Enum.Parse<LeaveStatus>(leave.Status);
                    _context.SaveChanges();
                    _logger.LogInformation($"[{nameof(DLeave)}] - [{nameof(UpdateLeave)}] - Leave with Id {leave?.Id} updated.");
                    return true;
                }
            }
            catch (ValidationException ex)
            {
                _logger.LogError($"[{nameof(DLeave)}] - [{nameof(UpdateLeave)}] - Validation Error while update leave: {ex}");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(DLeave)}] - [{nameof(UpdateLeave)}] - Error while update leave: {ex}");
                throw ex;
            }
        }
        public Leave? GetLeave(Guid id)
        {
            try
            {
                var leaveDTO = _context.Leaves.FirstOrDefault(l => l.Id == id);
                if (leaveDTO == null)
                {
                    Console.WriteLine($"leave with id = {id} not found");
                    _logger.LogInformation($"leave with id = {id} not found");
                    return null;
                }
                var leave = new Leave
                {
                    Id= leaveDTO.Id,
                    Description= leaveDTO.Description,
                    EmployeeEmail= leaveDTO.EmployeeEmail,
                    StartDate= leaveDTO.StartDate,
                    EndDate= leaveDTO.EndDate,
                    Status = leaveDTO.Status.ToString()
                };
                _logger.LogInformation($"[{nameof(DLeave)}] - [{nameof(GetLeave)}] - Retrieved leave id = {id}");
                return leave;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(DLeave)}] - [{nameof(GetLeave)}] - Error while retrieving leaves for employee: {ex}");
                throw ex;
            }
        }
        public List<Leave>? GetLeaves(string employeeEmail)
        {
            try
            {
                var leaves = _context.Leaves
                    .Where(l => l.EmployeeEmail == employeeEmail)
                    .Select(l => new Leave
                    {
                        Id = l.Id,
                        EmployeeEmail = l.EmployeeEmail,
                        Description = l.Description,
                        StartDate = l.StartDate,
                        EndDate = l.EndDate,
                        Status = l.Status.ToString(),
                    })
                    .ToList();

                _logger.LogInformation($"[{nameof(DLeave)}] - [{nameof(GetLeaves)}] - Retrieved leaves for employee with email {employeeEmail}");
                return leaves;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(DLeave)}] - [{nameof(GetLeaves)}] - Error while retrieving leaves for employee: {ex}");
                throw ex;
            }
        }
        public async Task<List<Leave>> GetLeaves()
        {
            try
            {
                var leaves = _context.Leaves
                    .Select(l => new Leave
                    {
                        Id = l.Id,
                        EmployeeEmail = l.EmployeeEmail,
                        Description = l.Description,
                        StartDate = l.StartDate,
                        EndDate = l.EndDate,
                        Status = l.Status.ToString(),
                    })?.OrderBy(l => l.StartDate)?
                    .ToList();

                _logger.LogInformation($"[{nameof(DLeave)}] - [{nameof(GetLeaves)}] - Retrieved all leaves");
                return leaves;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(DLeave)}] - [{nameof(GetLeave)}] - Error while retrieving leaves: {ex}");
                throw ex;
            }
        }
        public async Task<List<Leave>> GetApprovedLeaves()
        {
            try
            {
                var leaves = _context.Leaves.
                    Where(l => l.Status == LeaveStatus.Approved)
                   .Select(l => new Leave
                   {
                       Id = l.Id,
                       EmployeeEmail = l.EmployeeEmail,
                       Description = l.Description,
                       StartDate = l.StartDate,
                       EndDate = l.EndDate,
                   })
                   .ToList();
                _logger.LogInformation($"[{nameof(DLeave)}] - [{nameof(GetApprovedLeaves)}] - Retrived approved leaves");
                return leaves;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(DLeave)}] - [{nameof(GetApprovedLeaves)}] - Error while retrive approved leaves: {ex}");
                throw ex;
            }
        }
        public List<Leave>? GetPendingLeaves(string departmentName)
        {
            try
            {
                var leaves = (from emp in _context.Employees
                                 join leave in _context.Leaves on emp.UserEmail equals leave.EmployeeEmail
                                 where (leave.Status == LeaveStatus.Pending && emp.DepartmentName == departmentName)
                                 select new Leave
                                 {
                                     Id = leave.Id,
                                     EmployeeEmail = leave.EmployeeEmail,
                                     Description = leave.Description,
                                     StartDate = leave.StartDate,
                                     EndDate = leave.EndDate,
                                     Status = leave.Status.ToString(),
                                 }).ToList();

                _logger.LogInformation($"[{nameof(DLeave)}] - [{nameof(GetPendingLeaves)}] - Retrieved leaves that status pending for department {departmentName}");
                return leaves;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(DLeave)}] - [{nameof(GetPendingLeaves)}] - Error while retrieving pending leaves for department: {ex}");
                throw ex;
            }
        }
        public List<Leave>? GetLeavesForDepartment(string departmentName)
        {
            try
            {
                var leaves = (from emp in _context.Employees
                                 join leave in _context.Leaves on emp.UserEmail equals leave.EmployeeEmail
                                 where (emp.DepartmentName == departmentName)
                                 select new Leave
                                 {
                                     Id = leave.Id,
                                     EmployeeEmail = leave.EmployeeEmail,
                                     Description = leave.Description,
                                     StartDate = leave.StartDate,
                                     EndDate = leave.EndDate,
                                     Status = leave.Status.ToString(),
                                 }).ToList();

                _logger.LogInformation($"[{nameof(DLeave)}] - [{nameof(GetLeavesForDepartment)}] - Retrieved leaves for department {departmentName}");
                return leaves;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(DLeave)}] - [{nameof(GetLeavesForDepartment)}] - Error while retrieving leaves for department: {ex}");
                throw ex;
            }
        }
        private void ValidateLeave(Leave leave)
        {
            var validationContext = new ValidationContext(leave, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();

            if (!Validator.TryValidateObject(leave, validationContext, validationResults, validateAllProperties: true) || !Enum.TryParse<LeaveStatus>(leave.Status, out LeaveStatus status))
            {
                var errorMessages = validationResults.Select(result => result?.ErrorMessage);
                throw new ValidationException(string.Join(Environment.NewLine, errorMessages));
            }
        }
    }
}
