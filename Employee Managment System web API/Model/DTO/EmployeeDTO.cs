using System.ComponentModel.DataAnnotations;

namespace Employee_Management_System.Model
{
    public class EmployeeDTO
    {
        [Key]
        public string UserEmail { get; set; }
        [Required]
        public string DepartmentName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public UserDTO User { get; set; }
        public DepartmentDTO Department { get; set; }
        public SalaryDTO Salary { get; set; }
        public List<LeaveDTO> Leaves { get; set; }
        public List<AttendanceDTO> Attendances { get; set; }
    }
}
