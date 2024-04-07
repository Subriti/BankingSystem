using BankingSystem.API.Data.DbContext;
using BankingSystem.API.Data.Repository.IRepository;
using BankingSystem.API.DTOs;
using BankingSystem.API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace BankingSystem.API.Data.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher<Users> _passwordHasher;

        public UserRepository(ApplicationDbContext context, IPasswordHasher<Users> passwordHasher)
        {
            _context = context ?? throw new ArgumentOutOfRangeException(nameof(context));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        }
        public async Task<Users?> GetUserAsync(Guid Id)
        {
            //returns only user detail
            return await _context.SystemUser.Where(u => u.Id == Id).FirstOrDefaultAsync();
        }
        public async Task<Users?> GetUserByEmailAsync(string email)
        {
            //returns only user detail
            return await _context.SystemUser.Where(u => u.Email == email).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Users>> GetUsersAsync()
        {
            return await _context.SystemUser.OrderBy(c => c.Fullname).ToListAsync();
        }

        public async Task<IEnumerable<Users>> GetUsersByIdsAsync(List<Guid> userIds)
        {
            // Assuming you have DbSet<User> named Users in your DbContext
            var users = await _context.Users
                .Where(u => userIds.Contains(u.Id))
                .ToListAsync();

            return users;
        }

        public async Task<Users> AddUsers(Users users)
        {
            var user = _context.SystemUser.Add(users);
            await _context.SaveChangesAsync();

            return GetUserAsync(user.Entity.Id).Result;
        }

        public void DeleteUser(Guid userId)
        {
            var user = GetUserAsync(userId);
            _context.SystemUser.Remove(user.Result);
            _context.SaveChangesAsync();
        }

        public async Task<Users> PatchUserDetails(Guid Id, JsonPatchDocument<UserCreationDTO> patchDocument)
        {
            var existingUser = await GetUserAsync(Id);
            if (existingUser != null)
            {
                //transform user entity to usercreationDTO
                var userToPatch = new UserCreationDTO(existingUser.UserName, existingUser.Fullname, existingUser.Email, existingUser.PasswordHash, existingUser.Address, existingUser.DateOfBirth);

                patchDocument.ApplyTo(userToPatch);

                existingUser.UserName = userToPatch.UserName;
                existingUser.Fullname = userToPatch.Fullname;
                existingUser.Email = userToPatch.Email;

                string hashedPassword = _passwordHasher.HashPassword(existingUser, userToPatch.Password);

                existingUser.PasswordHash = hashedPassword;
                existingUser.Address = userToPatch.Address;
                existingUser.DateOfBirth = userToPatch.DateOfBirth;

                //update modifiedAt DateTime
                existingUser.ModifiedAt = DateTime.UtcNow;

                _context.SaveChangesAsync();
                return existingUser;
            }
            return null;
        }

        public async Task<Users> UpdateUsersAsync(Users finalUser)
        {
            var existingUser = await GetUserAsync(finalUser.Id);

            if (existingUser == null)
                return null;

            foreach (var property in typeof(Users).GetProperties())
            {
                if (property.CanWrite && property.Name != "Id" && property.GetValue(finalUser) != null)
                {
                    var newValue = property.GetValue(finalUser);
                    var existingValue = property.GetValue(existingUser);

                    if (!newValue.Equals(existingValue))
                    {
                        property.SetValue(existingUser, newValue);
                    }
                }
            }

            existingUser.ModifiedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return existingUser;
        }

        public async Task<Users> UpdatePasswordAsync(Users finalUser)
        {
            var existingUser = await GetUserAsync(finalUser.Id);
            if (existingUser != null)
            {
                if (!string.IsNullOrEmpty(finalUser.PasswordHash) && existingUser.PasswordHash != finalUser.PasswordHash)
                    existingUser.PasswordHash = finalUser.PasswordHash;

                //update modifiedAt DateTime
                existingUser.ModifiedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return existingUser;
            }
            return null;
        }
    }
}