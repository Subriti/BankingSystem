﻿using BankingSystem.API.DTO;
using BankingSystem.API.Models;
using Microsoft.AspNetCore.JsonPatch;

namespace BankingSystem.API.IRepository
{
    public interface IUserRepository
    {
        Task<IEnumerable<Users>> GetUsersAsync();
        Task<Users?> GetUserAsync(Guid Id);
        Task<Users?> GetUserByEmailAsync(string email);
        Task<Users> AddUsers(Users users);
        Task<Users> UpdateUsersAsync(Guid Id, Users users);
        void DeleteUser(Guid Id);
        Task<Users> PatchUserDetails(Guid Id, JsonPatchDocument<UserDTO> userDetails);
    }
}
