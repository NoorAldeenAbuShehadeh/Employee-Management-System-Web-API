using Employee_Management_System.DAL;
using Employee_Management_System.Model;
using Employee_Management_System.Services;
using Employee_Managment_System_web_API.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:5260");
                      });
});

var enviroment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile(enviroment == "Development" ? $"appSettings.Development.json" : $"appSettings.json").Build();
string connectionString = config.GetConnectionString("DefaultConnection");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = config["Jwt:Issuer"],
        ValidAudience = config["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]))
    };
    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = context =>
        {
            var userEmail = context.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var userRole = context.Principal.FindFirst(ClaimTypes.Role)?.Value;
            context.HttpContext.Items["CurrentUser"] = new User {Email = userEmail, Role = userRole };
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

builder.Services.AddSingleton(config)
                .AddLogging(se => se.AddLog4Net())
                .AddSingleton(new AppDbContext(connectionString))
                .AddSingleton<IDUsers, DUsers>()
                .AddSingleton<IDEmployees, DEmployees>()
                .AddSingleton<IDDepartments, DDepartments>()
                .AddSingleton<IDSalary, DSalary>()
                .AddSingleton<IDAttendance, DAttendance>()
                .AddSingleton<IDLeave, DLeave>()
                .AddSingleton<IDCommitDBChanges, DCommitDBChanges>()
                .AddSingleton<ILoginServices, LoginServices>()
                .AddSingleton<IAttendanceServices, AttendanceServices>()
                .AddSingleton<IDepartmentServices, DepartmentServices>()
                .AddSingleton<IEmployeeServices, EmployeeServices>()
                .AddSingleton<ILeaveServices, LeaveServices>()
                .AddSingleton<ISalaryServices, SalaryServices>();
 
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();

app.UseAuthorization();

app.UseMiddleware<UserMiddleware>();

app.MapControllers();

app.Run();

//return leaves.Any() ? leaves : null; add it to each get items