using Employee_Management_System.Model;

namespace Employee_Management_System.DAL
{
    public interface IDUsers
    {
        public Task<bool> AddUser(User user);
        public Task<bool> UpdateUser(User user);
        public Task<List<User>>? GetUsers();
        public Task<User>? GetUser(string email);
        public Task<bool> DeleteUser(string email);
        public Task<User> LogIn(string email, string password);
        public string? EncodePassword(string password);   
    }
}
