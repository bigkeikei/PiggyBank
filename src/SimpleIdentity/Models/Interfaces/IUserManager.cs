﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleIdentity.Models
{
    public interface IUserManager
    {
        Task<IEnumerable<User>> ListUsers();
        Task<User> CreateUser(User user);
        Task<User> FindUser(int userId);
        Task<User> FindUserByName(string userName);
        Task<User> FindUserByToken(string accessToken);
        Task<User> UpdateUser(User user);
        Task<string> GenerateNonce(int userId);
    }
}
