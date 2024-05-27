﻿using Repositories.DTO;
using Repositories.Entities;

namespace Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User> AddUser(UserSignupModel user, String role);
        Task<User> ChangeUserPasswordAsync(string id, string currentPassword, string newPassword);
        Task<bool> ConfirmEmail(string email, string token);
        Task<string> GenerateEmailConfirmationToken(User user);
        Task<string> GenerateTokenForResetPassword(User user);
        Task<User> GetUserByEmailAsync(string email);
        Task<List<User>> GetAllUsersAsync();
        Task<List<UserDetailsModel>> GetAllUsersWithRoleAsync();
        Task<List<string>> GetRoleName(User user);
        Task<ResponseLoginModel> LoginByEmailAndPassword(UserLoginModel user);
        Task<User> GetCurrentUserAsync();
        Task<User> GetAccountDetailsAsync(int userId);
        Task<User> UpdateAccountAsync(User user);
        Task<bool> UpdateUserRole(User user, string role);
    }
}
