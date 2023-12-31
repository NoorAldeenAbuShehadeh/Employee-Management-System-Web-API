using System.ComponentModel.DataAnnotations;

namespace Employee_Management_System.Model
{
    public class UserDTO
    {
        [Key]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Role { get; set; }
        [Required]
        public string Status { get; set; }
        public EmployeeDTO Employee { get; set; }
    }
}
