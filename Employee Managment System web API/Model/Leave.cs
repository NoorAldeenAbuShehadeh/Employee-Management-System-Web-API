using System.ComponentModel.DataAnnotations;

namespace Employee_Management_System.Model
{
    public class Leave
    {
        [Key]
        public Guid Id { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string? EmployeeEmail { get; set; }

        [MaxLength(255, ErrorMessage = "Description must not exceed 255 characters")]
        public string? Description { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Invalid date format for Start Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? StartDate { get; set; }
        
        [DataType(DataType.Date, ErrorMessage = "Invalid date format for End Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? EndDate { get; set; }

        public string? Status { get; set; }
    }
}
