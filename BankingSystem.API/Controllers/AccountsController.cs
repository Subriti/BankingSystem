﻿using BankingSystem.API.DTOs;
using BankingSystem.API.Entities;
using BankingSystem.API.Services;
using BankingSystem.API.Services.IServices;
using BankingSystem.API.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystem.API.Controllers
{
    /// <summary>
    /// Controller for handling accounts endpoints.
    /// </summary>
    [ApiController]
    [Route("api/accounts")]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService accountServices;
        private readonly IUserService userServices;
        private readonly IEmailService emailService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountsController"/> class.
        /// </summary>
        /// <param name="accountServices">The account services.</param>
        /// <param name="userServices">The user services.</param>
        /// <param name="_emailService">The email service.</param>
        public AccountsController(IAccountService accountServices, IUserService userServices, IEmailService _emailService)
        {
            this.accountServices = accountServices ?? throw new ArgumentNullException(nameof(accountServices));
            this.userServices = userServices ?? throw new ArgumentNullException(nameof(userServices));
            this.emailService = _emailService;

        }

        /// <summary>
        /// Gets all accounts.
        /// </summary>
        /// <returns>A list of <see cref="Accounts"/>.</returns>
        [HttpGet]
        [CustomAuthorize("TellerPerson")]
        public async Task<ActionResult<IEnumerable<Accounts>>> GetAccounts()
        {
            var accounts = await accountServices.GetAccountsAsync();
            if (accounts == null)
            {
                return Ok(new List<Accounts>());
            }

            return Ok(accounts);
        }

        /// <summary>
        /// Gets an account by id.
        /// </summary>
        /// <param name="id">The id of the account.</param>
        /// <returns>The <see cref="Accounts"/>.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Accounts>> GetAccountAsync(Guid id)
        {
            var account = await accountServices.GetAccountAsync(id);
            if (account == null)
            {
                return NotFound();
            }
            return Ok(account);
        }

        /// <summary>
        /// Deletes an account by id.
        /// </summary>
        /// <param name="accountId">The id of the account.</param>
        /// <returns>A NoContent response.</returns>
        [HttpDelete("{accountId}")]
        [CustomAuthorize("TellerPerson")]
        public ActionResult DeleteUser(Guid accountId)
        {
            accountServices.DeleteAccount(accountId);
            return NoContent();
        }

        /// <summary>
        /// Updates an account pin by user email .
        /// </summary>
        /// <param name="updateModel">The update model.</param>
        /// <param name="email">The email of the user.</param>
        /// <returns>The updated <see cref="Accounts"/>.</returns>
        [HttpPut]
        [CustomAuthorize("AccountHolder")]
        public async Task<ActionResult<Accounts>> UpdateAccounts(AccountUpdateDTO updateModel, string email)
        {
            var user = await userServices.GetUserByEmailAsync(email);

            if (user == null)
            {
                return NotFound("User not found");
            }

            var userId = user.Id;

            var checkAccount = await accountServices.GetAccountByUserIdAsync(userId);
            if (checkAccount == null)
            {
                return NotFound("User account does not exist");
            }

            var accountId = checkAccount.AccountId;

            var newAccount = await accountServices.UpdateAccountsAsync(accountId, updateModel);
            if (newAccount == null)
            {
                return BadRequest("Update failed");
            }
            return Ok(newAccount);
        }
    }
}
