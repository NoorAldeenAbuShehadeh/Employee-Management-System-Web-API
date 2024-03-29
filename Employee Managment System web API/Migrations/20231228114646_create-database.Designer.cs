﻿// <auto-generated />
using System;
using Employee_Management_System.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Employee_Managment_System_web_API.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20231228114646_create-database")]
    partial class createdatabase
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.25")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Employee_Management_System.Model.AttendanceDTO", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<DateTime?>("CheckIn")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("CheckOut")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("EmployeeEmail")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("EmployeeEmail");

                    b.ToTable("Attendances");
                });

            modelBuilder.Entity("Employee_Management_System.Model.DepartmentDTO", b =>
                {
                    b.Property<string>("Name")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("ManagerEmail")
                        .HasColumnType("varchar(255)");

                    b.HasKey("Name");

                    b.HasIndex("ManagerEmail")
                        .IsUnique();

                    b.ToTable("Departments");
                });

            modelBuilder.Entity("Employee_Management_System.Model.EmployeeDTO", b =>
                {
                    b.Property<string>("UserEmail")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("DepartmentName")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("UserEmail");

                    b.HasIndex("DepartmentName");

                    b.ToTable("Employees");
                });

            modelBuilder.Entity("Employee_Management_System.Model.LeaveDTO", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("EmployeeEmail")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("EmployeeEmail");

                    b.ToTable("Leaves");
                });

            modelBuilder.Entity("Employee_Management_System.Model.SalaryDTO", b =>
                {
                    b.Property<string>("EmployeeEmail")
                        .HasColumnType("varchar(255)");

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("Bonuses")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("Deductions")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("EmployeeEmail");

                    b.ToTable("Salaries");
                });

            modelBuilder.Entity("Employee_Management_System.Model.UserDTO", b =>
                {
                    b.Property<string>("Email")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Email");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Employee_Management_System.Model.AttendanceDTO", b =>
                {
                    b.HasOne("Employee_Management_System.Model.EmployeeDTO", "Employee")
                        .WithMany("Attendances")
                        .HasForeignKey("EmployeeEmail")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Employee");
                });

            modelBuilder.Entity("Employee_Management_System.Model.DepartmentDTO", b =>
                {
                    b.HasOne("Employee_Management_System.Model.EmployeeDTO", "Manager")
                        .WithOne()
                        .HasForeignKey("Employee_Management_System.Model.DepartmentDTO", "ManagerEmail");

                    b.Navigation("Manager");
                });

            modelBuilder.Entity("Employee_Management_System.Model.EmployeeDTO", b =>
                {
                    b.HasOne("Employee_Management_System.Model.DepartmentDTO", "Department")
                        .WithMany("Employees")
                        .HasForeignKey("DepartmentName")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Employee_Management_System.Model.UserDTO", "User")
                        .WithOne("Employee")
                        .HasForeignKey("Employee_Management_System.Model.EmployeeDTO", "UserEmail");

                    b.Navigation("Department");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Employee_Management_System.Model.LeaveDTO", b =>
                {
                    b.HasOne("Employee_Management_System.Model.EmployeeDTO", "Employee")
                        .WithMany("Leaves")
                        .HasForeignKey("EmployeeEmail")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Employee");
                });

            modelBuilder.Entity("Employee_Management_System.Model.SalaryDTO", b =>
                {
                    b.HasOne("Employee_Management_System.Model.EmployeeDTO", "Employee")
                        .WithOne("Salary")
                        .HasForeignKey("Employee_Management_System.Model.SalaryDTO", "EmployeeEmail")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Employee");
                });

            modelBuilder.Entity("Employee_Management_System.Model.DepartmentDTO", b =>
                {
                    b.Navigation("Employees");
                });

            modelBuilder.Entity("Employee_Management_System.Model.EmployeeDTO", b =>
                {
                    b.Navigation("Attendances");

                    b.Navigation("Leaves");

                    b.Navigation("Salary")
                        .IsRequired();
                });

            modelBuilder.Entity("Employee_Management_System.Model.UserDTO", b =>
                {
                    b.Navigation("Employee")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
