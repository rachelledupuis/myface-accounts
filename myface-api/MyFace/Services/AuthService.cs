using System;
using MyFace.Helpers;
using MyFace.Repositories;
using MyFace.Models.Database;

namespace MyFace.Services
{
    public interface IAuthService
    {
        bool IsValidUsernameAndPassword(string username, string password);
    }
    public class AuthService : IAuthService
    {
        private readonly IUsersRepo _users;
        public AuthService(IUsersRepo users)
        {
            _users = users;
        }
        public bool IsValidUsernameAndPassword(string username, string password)
        {
            User user;
            try
            {
                user = _users.GetByUsername(username);
            }
            catch (InvalidOperationException e)
            {
                return false;
            }
            
            var salt = Convert.FromBase64String(user.Salt);
            var hashed = PasswordHelper.GetHashedPassword(password, salt);

            if (hashed.HashedPassword != user.HashedPassword)
            {
                return false;
            }
            return true;
        }
    }
}