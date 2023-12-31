using System.ComponentModel.DataAnnotations;

namespace Employee_Management_System.Model
{
    public class Salary
    {
        [Key]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string EmployeeEmail { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Amount must be a non-negative value")]
        public decimal? Amount { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Bonuses must be a non-negative value")]
        public decimal? Bonuses { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Deductions must be a non-negative value")]
        public decimal? Deductions { get; set; }
    }
}
