using System.ComponentModel.DataAnnotations;

namespace Employee_Management_System.Model
{
    public class Department
    {
        [Key]
        [MinLength(2, ErrorMessage = "Department Name must be at least 2 characters")]
        public string? Name { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string? ManagerEmail { get; set; }
    }
}
