using BankingSystem.API.DTOs;
using BankingSystem.API.Entities;
using Microsoft.AspNetCore.JsonPatch;

namespace BankingSystem.API.Data.Repository.IRepository
{
    public interface IUserRepository
    {
        Task<IEnumerable<Users>> GetUsersAsync();
        Task<Users?> GetUserAsync(Guid Id);
        Task<IEnumerable<Users>> GetUsersByIdsAsync(List<Guid> userIds);
        Task<Users?> GetUserByEmailAsync(string email);
        Task<Users> AddUsers(Users users);
        Task<Users> UpdateUsersAsync(Users finalUser);
        Task<Users> UpdatePasswordAsync(Users finalUser);
        void DeleteUser(Guid userId);
        Task<Users> PatchUserDetails(Guid Id, JsonPatchDocument<UserCreationDTO> patchDocument);
    }
}
