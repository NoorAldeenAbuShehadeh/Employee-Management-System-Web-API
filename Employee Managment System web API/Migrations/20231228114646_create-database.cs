using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Employee_Managment_System_web_API.Migrations
{
    public partial class createdatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Email = table.Column<string>(type: "varchar(255)", nullable: false),
                    Password = table.Column<string>(type: "longtext", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: false),
                    Role = table.Column<string>(type: "longtext", nullable: false),
                    Status = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Email);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Attendances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    EmployeeEmail = table.Column<string>(type: "varchar(255)", nullable: false),
                    CheckIn = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CheckOut = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attendances", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Name = table.Column<string>(type: "varchar(255)", nullable: false),
                    ManagerEmail = table.Column<string>(type: "varchar(255)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Name);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    UserEmail = table.Column<string>(type: "varchar(255)", nullable: false),
                    DepartmentName = table.Column<string>(type: "varchar(255)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "longtext", nullable: false),
                    Address = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.UserEmail);
                    table.ForeignKey(
                        name: "FK_Employees_Departments_DepartmentName",
                        column: x => x.DepartmentName,
                        principalTable: "Departments",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Employees_Users_UserEmail",
                        column: x => x.UserEmail,
                        principalTable: "Users",
                        principalColumn: "Email");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Leaves",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    EmployeeEmail = table.Column<string>(type: "varchar(255)", nullable: false),
                    Description = table.Column<string>(type: "longtext", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leaves", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Leaves_Employees_EmployeeEmail",
                        column: x => x.EmployeeEmail,
                        principalTable: "Employees",
                        principalColumn: "UserEmail",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Salaries",
                columns: table => new
                {
                    EmployeeEmail = table.Column<string>(type: "varchar(255)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Bonuses = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Deductions = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Salaries", x => x.EmployeeEmail);
                    table.ForeignKey(
                        name: "FK_Salaries_Employees_EmployeeEmail",
                        column: x => x.EmployeeEmail,
                        principalTable: "Employees",
                        principalColumn: "UserEmail",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_EmployeeEmail",
                table: "Attendances",
                column: "EmployeeEmail");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_ManagerEmail",
                table: "Departments",
                column: "ManagerEmail",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_DepartmentName",
                table: "Employees",
                column: "DepartmentName");

            migrationBuilder.CreateIndex(
                name: "IX_Leaves_EmployeeEmail",
                table: "Leaves",
                column: "EmployeeEmail");

            migrationBuilder.AddForeignKey(
                name: "FK_Attendances_Employees_EmployeeEmail",
                table: "Attendances",
                column: "EmployeeEmail",
                principalTable: "Employees",
                principalColumn: "UserEmail",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_Employees_ManagerEmail",
                table: "Departments",
                column: "ManagerEmail",
                principalTable: "Employees",
                principalColumn: "UserEmail");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Departments_Employees_ManagerEmail",
                table: "Departments");

            migrationBuilder.DropTable(
                name: "Attendances");

            migrationBuilder.DropTable(
                name: "Leaves");

            migrationBuilder.DropTable(
                name: "Salaries");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
