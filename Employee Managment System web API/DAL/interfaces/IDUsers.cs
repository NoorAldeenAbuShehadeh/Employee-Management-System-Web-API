﻿using Employee_Management_System.Model;

namespace Employee_Management_System.DAL
{
    public interface IDUsers
    {
        public Task<bool> AddUser(User user);
        public bool UpdateUser(User user);
        public List<User>? GetUsers();
        public User? GetUser(string email);
        public bool DeleteUser(string email);
        public Task<User> LogIn(string email, string password);
        public string? EncodePassword(string password);   
    }
}