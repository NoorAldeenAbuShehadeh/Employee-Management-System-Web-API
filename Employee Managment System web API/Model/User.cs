using System.ComponentModel.DataAnnotations;

namespace Employee_Management_System.Model
{
    public class User
    {
        [Key]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string? Password { get; set; }

        [MinLength(5, ErrorMessage = "Name must be at least 5 characters")]
        public string? Name { get; set; }

        [RegularExpression("^(manager|employee)$", ErrorMessage = "Role must be 'manager' or 'employee'")]
        public string? Role { get; set; }

        [RegularExpression("^(active|inActive)$", ErrorMessage = "Status must be 'active' or 'inActive'")]
        public string? Status { get; set; }
    }
}
