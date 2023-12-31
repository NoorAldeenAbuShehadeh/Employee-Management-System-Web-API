using System.ComponentModel.DataAnnotations;

namespace Employee_Management_System.Model
{
    public class Attendance
    {
        [Key]
        public Guid Id { get; set; }

        //[Required(ErrorMessage = "Employee email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string? EmployeeEmail { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Invalid date format for Check In")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? CheckIn { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Invalid date format for Check Out")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? CheckOut { get; set; }
        
        public string? Status { get; set; }
    }
}
