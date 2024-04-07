﻿using BankingSystem.API.Data.DbContext;
using BankingSystem.API.Data.Repository.IRepository;
using BankingSystem.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace BankingSystem.API.Data.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ApplicationDbContext _context;
        public AccountRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentOutOfRangeException(nameof(context));
        }
        public async Task<Accounts?> GetAccountAsync(Guid accountId)
        {
            //returns only account detail
            return await _context.Account.Where(a => a.AccountId == accountId).FirstOrDefaultAsync();
        }
        public async Task<IEnumerable<Accounts>> GetAccountsAsync()
        {
            //return await _context.Users.OrderBy(c => c.Name).ToListAsync();
            return await _context.Account.OrderBy(a => a.AccountNumber).ToListAsync();
        }

        public async Task<IEnumerable<Accounts>> GetAccountsByIdsAsync(List<Guid> accountIds)
        {
            // Assuming you have DbSet<Account> named Accounts in your DbContext
            var accounts = await _context.Account
                .Where(a => accountIds.Contains(a.AccountId))
                .ToListAsync();

            return accounts;
        }

        public async Task<Accounts?> GetAccountByAccountNumberAsync(long accountNumber)
        {
            return await _context.Account.Where(a => a.AccountNumber == accountNumber).FirstOrDefaultAsync();
        }
        public async Task<Accounts?> GetAccountByUserIdAsync(Guid userId)
        {
            return await _context.Account.Where(a => a.UserId == userId).FirstOrDefaultAsync();
        }


        public async Task<Accounts> AddAccounts(Accounts accounts)
        {
            var account = _context.Account.Add(accounts);
            await _context.SaveChangesAsync();

            return await GetAccountAsync(account.Entity.AccountId);
        }

        public void DeleteAccount(Guid accountId)
        {
            var account = GetAccountAsync(accountId);
            _context.Account.Remove(account.Result);
            _context.SaveChangesAsync();
        }

        public async Task<Accounts> UpdateAccountsAsync(Guid accountId, Accounts accounts)
        {
            var existingAccount = await GetAccountAsync(accountId);
            if (existingAccount != null)
            {
                existingAccount.AtmCardPin = accounts.AtmCardPin;

                _context.SaveChanges();
                return existingAccount;
            }
            return null;
        }
    }
}
