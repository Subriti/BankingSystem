﻿using BankingSystem.API.Models;
using Microsoft.AspNetCore.Identity;
using System.Globalization;

namespace BankingSystem.API.DbContext
{
    public static class AppDBInitialize
    {
        public static async Task SeedConstantsAsync(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<Users>>();

                // Seed Roles
                await SeedRoleAsync(roleManager, UserRoles.AccountHolder);
                await SeedRoleAsync(roleManager, UserRoles.TellerPerson);

               // Save Changes
                var context = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedRoleAsync(RoleManager<IdentityRole<Guid>> roleManager, string roleName)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
                await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
        }

    }
}

