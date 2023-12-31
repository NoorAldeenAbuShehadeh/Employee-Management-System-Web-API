namespace Employee_Management_System.Model
{
    public class EmployeeCreationModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public string DepartmentName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public decimal SalaryAmount { get; set; }
        public decimal SalaryBonuses { get; set; }
        public decimal SalaryDeductions { get; set; }
    }
}
