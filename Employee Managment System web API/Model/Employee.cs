using System.ComponentModel.DataAnnotations;

namespace Employee_Management_System.Model
{
    public class Employee
    {
        [Key]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string? UserEmail { get; set; }

        [MinLength(2, ErrorMessage = "Department Name must be at least 2 characters")]
        public string? DepartmentName { get; set; }

        [Phone(ErrorMessage = "Invalid phone number format")]
        public string? PhoneNumber { get; set; }

        [MinLength(5, ErrorMessage = "Address must be at least 5 characters")]
        public string? Address { get; set; }
    }
}
