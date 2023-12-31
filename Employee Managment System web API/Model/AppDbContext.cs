using Microsoft.EntityFrameworkCore;

namespace Employee_Management_System.Model
{
    public class AppDbContext : DbContext
    {
        private readonly string _connectionString;
        public DbSet<UserDTO> Users { get; set; }
        public DbSet<EmployeeDTO> Employees { get; set; }
        public DbSet<DepartmentDTO> Departments { get; set; }
        public DbSet<SalaryDTO> Salaries { get; set; }
        public DbSet<LeaveDTO> Leaves { get; set; }
        public DbSet<AttendanceDTO> Attendances { get; set; }
        public AppDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL(_connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Primary keys for tables

            modelBuilder.Entity<UserDTO>()
                .HasKey(u => u.Email);

            modelBuilder.Entity<EmployeeDTO>()
                .HasKey(e => e.UserEmail);

            modelBuilder.Entity<DepartmentDTO>()
                .HasKey(d => d.Name);

            modelBuilder.Entity<SalaryDTO>()
                .HasKey(s => s.EmployeeEmail);

            modelBuilder.Entity<LeaveDTO>()
                .HasKey(l => l.Id);

            modelBuilder.Entity<AttendanceDTO>()
                .HasKey(a => a.Id);

            // Configure relationships between tables

            // EmployeeDTO - UserDTO *** one - one relationship
            modelBuilder.Entity<EmployeeDTO>()
                .HasOne(e => e.User)
                .WithOne(u => u.Employee)
                .HasForeignKey<EmployeeDTO>(e => e.UserEmail)
                .IsRequired(false);// because not all user is employee ==> there is admin

            // DepartmentDTO - ManagerDTO (Employee) *** one - one relationship
            modelBuilder.Entity<DepartmentDTO>()
                .HasOne(d => d.Manager)
                .WithOne() // the manager can manage only one depertment
                .HasForeignKey<DepartmentDTO>(d => d.ManagerEmail)
                .IsRequired(false);//can be null

            // DepartmentDTO - EmployeesDTO *** one - many relationship
            modelBuilder.Entity<DepartmentDTO>()
                .HasMany(d => d.Employees)
                .WithOne(e => e.Department)
                .HasForeignKey(e => e.DepartmentName);

            // SalaryDTO - EmployeeDTO *** one - one relationship
            modelBuilder.Entity<SalaryDTO>()
                .HasOne(s => s.Employee)
                .WithOne(e => e.Salary)
                .HasForeignKey<SalaryDTO>(s => s.EmployeeEmail);

            // LeaveDTO - EmployeeDTO *** many - one relationship
            modelBuilder.Entity<LeaveDTO>()
                .HasOne(l => l.Employee)
                .WithMany(e => e.Leaves)
                .HasForeignKey(l => l.EmployeeEmail);

            // AttendanceDTO - EmployeeDTO *** many - one relationship
            modelBuilder.Entity<AttendanceDTO>()
                .HasOne(a => a.Employee)
                .WithMany(e => e.Attendances)
                .HasForeignKey(a => a.EmployeeEmail);

            base.OnModelCreating(modelBuilder);
        }

    }
}
