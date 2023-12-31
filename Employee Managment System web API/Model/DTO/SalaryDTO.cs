using System.ComponentModel.DataAnnotations;

namespace Employee_Management_System.Model
{
    public class SalaryDTO
    {
        [Key]
        public string EmployeeEmail { get; set; }
        public decimal? Amount { get; set; }
        public decimal? Bonuses { get; set; }
        public decimal? Deductions { get; set; }
        public EmployeeDTO Employee { get; set; }
    }
}
