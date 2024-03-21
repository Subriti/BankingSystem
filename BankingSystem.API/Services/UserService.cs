﻿using BankingSystem.API.DTO;
using BankingSystem.API.IRepository;
using BankingSystem.API.Models;
using Microsoft.AspNetCore.JsonPatch;
using AutoMapper;
using System.Text;

namespace BankingSystem.API.Services
{
    public class UserService
    {
        private readonly IUserRepository UserRepository;
        private readonly AccountServices AccountServices;

        private readonly IMapper _mapper;
        public UserService(IUserRepository userRepository, IMapper mapper, AccountServices accountServices)
        {
            UserRepository = userRepository ?? throw new ArgumentOutOfRangeException(nameof(userRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            AccountServices = accountServices;
        }

        public async Task<Users?> GetUserAsync(Guid Id)
        {
            //returns only user detail
            return await UserRepository.GetUserAsync(Id);
        }
        public async Task<Users?> GetUserByEmailAsync(string email)
        {
            //returns only user detail
            return await UserRepository.GetUserByEmailAsync(email);
        }

        public async Task<IEnumerable<Users>> GetUsersAsync()
        {
            return await UserRepository.GetUsersAsync();
        }

        public async Task<Users> RegisterUsers(UserDTO users)
        {
            var finalUser = _mapper.Map<Users>(users);

            // Hash password using bcrypt
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(users.Password);
            finalUser.Password = hashedPassword;

            var SavedUser= await UserRepository.AddUsers(finalUser);

            //if user is accountHolder, create account
            if(SavedUser.UserType==UserRoles.AccountHolder)
            {
                var accountNumber = GenerateRandomAccountNumber(1);
                var atmCardNum = GenerateRandomAccountNumber(2);
                var atmCardPin = (int)GenerateRandomAccountNumber(3);

                var accountDTO = new AccountDTO(SavedUser.Id, accountNumber, 0, atmCardNum, atmCardPin, DateTime.UtcNow, SavedUser.Id, DateTime.UtcNow, SavedUser.Id);
                await AccountServices.AddAccounts(accountDTO);
            }
            return SavedUser;
        }

        public void DeleteUser(Guid Id)
        {
            UserRepository.DeleteUser(Id);
        }

        public async Task<Users> PatchUserDetails(Guid Id, JsonPatchDocument<UserDTO> patchDocument)
        {
            return await UserRepository.PatchUserDetails(Id, patchDocument);
        }

        public async Task<Users> UpdateUsersAsync(Guid Id, UserDTO users)
        {
            var finalUser = _mapper.Map<Users>(users);
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(users.Password);
            finalUser.Password = hashedPassword;

            return await UserRepository.UpdateUsersAsync(Id, finalUser);
        }

        public async Task<Users> LoginUser(string email, string password)
        {
            // Retrieve the hashed password for the given username/email from your user database
            var existingUser = await UserRepository.GetUserByEmailAsync(email);
            if (existingUser != null)
            {
                if (existingUser.Email.Equals(email) && BCrypt.Net.BCrypt.Verify(password, existingUser.Password))
                {
                    return existingUser;
                }
                else
                {
                    // Passwords don't match
                    return null;
                }
            }
            return null;
        }

        private static Random ran = new Random();
        private long GenerateRandomAccountNumber(int num)
        {
            switch (num)
            {
                case 1:
                    var accountNumber = new StringBuilder("100001");
                    while (accountNumber.Length < 16)
                    {
                        accountNumber.Append(ran.Next(10).ToString());
                    }
                    return long.Parse(accountNumber.ToString());
                case 2:
                    var atmCardNum = new StringBuilder("900009");
                    while (atmCardNum.Length < 16)
                    {
                        atmCardNum.Append(ran.Next(10).ToString());
                    }
                    return long.Parse(atmCardNum.ToString());
                case 3:
                    var atmCardPin = new StringBuilder("0");
                    while (atmCardPin.Length < 6)
                    {
                        atmCardPin.Append(ran.Next(10).ToString());
                    }
                    return long.Parse(atmCardPin.ToString());
                default:
                    return 0;
            }
        }
    }
}
