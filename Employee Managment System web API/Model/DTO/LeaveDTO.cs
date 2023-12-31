using System.ComponentModel.DataAnnotations;

namespace Employee_Management_System.Model
{
    public class LeaveDTO
    {
        [Key]
        public Guid Id { get; set; }
        public string EmployeeEmail { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public LeaveStatus Status { get; set; }
        public EmployeeDTO Employee { get; set; }
    }
}
