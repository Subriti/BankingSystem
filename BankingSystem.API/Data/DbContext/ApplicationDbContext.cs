using BankingSystem.API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BankingSystem.API.Data.DbContext
{
    public class ApplicationDbContext : IdentityDbContext<Users, IdentityRole<Guid>, Guid>
    {
        //Defining Constructor
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            //Database.EnsureCreated();
        }

        //Define Databases
        public DbSet<Users> SystemUser { get; set; }

        public DbSet<KycDocument> KycDocuments { get; set; }

        public DbSet<Accounts> Account { get; set; }

        public DbSet<Transaction> Transactions { get; set; }
    }
}