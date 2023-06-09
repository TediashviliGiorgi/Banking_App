﻿using System.ComponentModel.DataAnnotations;

namespace BankingSystem.Features.InternetBank.User.Transactions
{
    public class TransactionRequest
    {
        [RegularExpression(@"^\d{23}$", ErrorMessage = "Iban must be 23 characters long")]
        public string SenderAccountIBAN { get; set; }
        public string RecipientAccountIBAN { get; set; }

        [RegularExpression(@"^\d+$", ErrorMessage = "Amount must be a valid number")]
        [Range(typeof(decimal), "0.01", "1000000000000", ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }

    }
}
