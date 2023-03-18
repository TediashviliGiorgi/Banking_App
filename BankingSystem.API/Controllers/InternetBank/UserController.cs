﻿using BankingSystem.DB.Entities;
using BankingSystem.DB;
using BankingSystem.Features.InternetBank.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BankingSystem.Features.InternetBank.Operator.RegisterUser;
using Microsoft.EntityFrameworkCore;
using BankingSystem.Features.InternetBank.Operator.AuthUser;
using System.Security.Claims;
using BankingSystem.Features.InternetBank.User.GetUserInfo;
using BankingSystem.Features.InternetBank.User.Transactions;

namespace BankingSystem.Features.InternetBank.Operator.AddUser
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IGetUserInfoRepository _getUserInfoRepository;
        private readonly ITransactionService _transactionService;
        public UserController(IUserRepository userRepository, IGetUserInfoRepository getUserInfoRepository, ITransactionService transactionService)
        {
            _userRepository = userRepository;
            _getUserInfoRepository = getUserInfoRepository;
            _transactionService = transactionService;
        }

        [HttpPost("login-user")]
        public async Task<IActionResult> LoginUser([FromBody] UserLoginRequest request)
        {
           var response = await _userRepository.LoginUserAsync(request);

            return Ok(response);
        }

        [Authorize("ApiUser", AuthenticationSchemes = "Bearer")]
        [HttpGet("get-accounts")]
        public async Task<IActionResult> GetUserAccounts(int userId)
        {
            var authenticatedUserId = User.FindFirstValue("userId");

            if (authenticatedUserId == null || int.Parse(authenticatedUserId) != userId)
            {
                return NotFound();
            }

            var accounts = await _getUserInfoRepository.GetUserAccountsAsync(userId);
            return Ok(accounts);
        }

        [Authorize("ApiUser", AuthenticationSchemes = "Bearer")]
        [HttpGet("get-cards")]
        public async Task<IActionResult> GetUserCards(int userId)
        {
            var authenticatedUserId = User.FindFirstValue("userId");

            if (authenticatedUserId == null || int.Parse(authenticatedUserId) != userId)
            {
                return NotFound();
            }

            var cards = await _getUserInfoRepository.GetUserCardsAsync(userId);
            return Ok(cards);
        }

        [Authorize("ApiUser", AuthenticationSchemes = "Bearer")]
        [HttpGet("get-account-balance")]
        public async Task<IActionResult> GetUserBalance(string iban, int userId)
        {
            var authenticatedUserId = User.FindFirstValue("userId");

            if (authenticatedUserId == null || int.Parse(authenticatedUserId) != userId)
            {
                return NotFound("Invalid userId or account");
            }
            else
            {
                var balance = await _getUserInfoRepository.GetUserBalanceAsync(iban, userId);
                return Ok(balance);
            }

        }

        [Authorize("ApiUser", AuthenticationSchemes = "Bearer")]
        [HttpGet("get-account-transactions")]
        public async Task<IActionResult> GetUserAccountTransactions(string iban, int userId)
        {
            var authenticatedUserId = User.FindFirstValue("userId");

            if (authenticatedUserId == null || int.Parse(authenticatedUserId) != userId)
            {
                return NotFound("Invalid userId or account");
            }
            else
            {
                var transactions = await _getUserInfoRepository.GetUserAccountTransactionsAsync(iban, userId);
                return Ok(transactions);
            }
        }

        [Authorize("ApiUser", AuthenticationSchemes = "Bearer")]
        [HttpPost("money-transfer")]
        public async Task<IActionResult> TransactionFunds([FromBody] TransactionRequest transactionRequest)
        {
            var transaction = await _transactionService.TransferFunds(transactionRequest);

            return Ok(transaction);
        }


    }
}