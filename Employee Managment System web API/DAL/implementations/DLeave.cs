using Employee_Management_System.Model;
using Microsoft.EntityFrameworkCore;
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
                    _logger.LogError($"[{nameof(DLeave)}] - [{nameof(AddLeave)}] - Bad Request should has a leave to add it");
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
                    await _context.Leaves.AddAsync(leaveDTO);
                    await _context.SaveChangesAsync();
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
        public async Task<bool> UpdateLeave(Leave leave)
        {
            try
            {
                LeaveDTO? leaveDTO = await _context.Leaves.FirstOrDefaultAsync(l => l.Id == leave.Id);
                if (leaveDTO == null)
                {
                    _logger.LogError($"[{nameof(DLeave)}] - [{nameof(UpdateLeave)}] - leave with Id {leave?.Id} not found.");
                    return false;
                }
                else
                {
                    ValidateLeave(leave);
                    leaveDTO.StartDate = leave.StartDate;
                    leaveDTO.EndDate = leave.EndDate;
                    leaveDTO.Description = leave.Description;
                    leaveDTO.Status = Enum.Parse<LeaveStatus>(leave.Status);
                    await _context.SaveChangesAsync();
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
        public async Task<Leave>? GetLeave(Guid id)
        {
            try
            {
                var leaveDTO = await _context.Leaves.FirstOrDefaultAsync(l => l.Id == id);
                if (leaveDTO == null)
                {
                    _logger.LogInformation($"[{nameof(DLeave)}] - [{nameof(GetLeave)}] - Leave with id = {id} not found");
                    return null;
                }
                var leave = new Leave
                {
                    Id = leaveDTO.Id,
                    Description = leaveDTO.Description,
                    EmployeeEmail = leaveDTO.EmployeeEmail,
                    StartDate = leaveDTO.StartDate,
                    EndDate = leaveDTO.EndDate,
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
        public async Task<List<Leave>>? GetLeaves(string employeeEmail)
        {
            try
            {
                var leaves = await _context.Leaves
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
                    .ToListAsync();

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
                var leaves = await _context.Leaves
                    .Select(l => new Leave
                    {
                        Id = l.Id,
                        EmployeeEmail = l.EmployeeEmail,
                        Description = l.Description,
                        StartDate = l.StartDate,
                        EndDate = l.EndDate,
                        Status = l.Status.ToString(),
                    }).OrderBy(l => l.StartDate)
                    .ToListAsync();

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
                var leaves = await _context.Leaves.
                    Where(l => l.Status == LeaveStatus.Approved)
                   .Select(l => new Leave
                   {
                       Id = l.Id,
                       EmployeeEmail = l.EmployeeEmail,
                       Description = l.Description,
                       StartDate = l.StartDate,
                       EndDate = l.EndDate,
                   })
                   .ToListAsync();
                _logger.LogInformation($"[{nameof(DLeave)}] - [{nameof(GetApprovedLeaves)}] - Retrived approved leaves");
                return leaves;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(DLeave)}] - [{nameof(GetApprovedLeaves)}] - Error while retrive approved leaves: {ex}");
                throw ex;
            }
        }
        public async Task<List<Leave>>? GetPendingLeaves(string departmentName)
        {
            try
            {
                var leaves = await (from emp in _context.Employees
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
                                    }).ToListAsync();

                _logger.LogInformation($"[{nameof(DLeave)}] - [{nameof(GetPendingLeaves)}] - Retrieved leaves that status pending for department {departmentName}");
                return leaves;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(DLeave)}] - [{nameof(GetPendingLeaves)}] - Error while retrieving pending leaves for department: {ex}");
                throw ex;
            }
        }
        public async Task<List<Leave>>? GetLeavesForDepartment(string departmentName)
        {
            try
            {
                var leaves = await (from emp in _context.Employees
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
                                    }).ToListAsync();

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
