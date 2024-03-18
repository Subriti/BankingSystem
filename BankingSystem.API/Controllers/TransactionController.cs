﻿using BankingSystem.API.Models;
using BankingSystem.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystem.API.Controllers
{
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly TransactionServices _transactionServices;

        public TransactionController(TransactionServices transactionServices)
        {
            _transactionServices = transactionServices ?? throw new ArgumentOutOfRangeException(nameof(transactionServices));
        }


        [Route("api/transactions")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactions(int accountId)
        {
            if (await _transactionServices.GetTransactionsOfAccountAsync(accountId) == null)
            {
                var list = new List<Transaction>();
                return list;
            }

            return Ok(await _transactionServices.GetTransactionsOfAccountAsync(accountId));
        }

        [Route("api/transactions")]
        [HttpDelete]
        public async Task<ActionResult> DeleteTransaction(int accountId, int transactionId)
        {
            if (!await _transactionServices.TransactionExistAsync(transactionId))
            {
                return NotFound();
            }
             _transactionServices.DeleteTransaction(accountId, transactionId);

            return NoContent();
        }
    }
}