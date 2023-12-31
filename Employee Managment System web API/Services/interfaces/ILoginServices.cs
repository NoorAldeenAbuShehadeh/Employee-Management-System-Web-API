using Employee_Management_System.Model;

namespace Employee_Management_System.Services
{
    public interface ILoginServices
    {
        public Task<string> Authenticate(User user);
    }
}
