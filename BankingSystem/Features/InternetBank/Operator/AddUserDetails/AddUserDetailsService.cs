﻿using BankingSystem.DB.Entities;
using BankingSystem.Features.InternetBank.Operator.AddAccountForUser;
using BankingSystem.Features.InternetBank.Operator.AddUser;
using IbanNet;

namespace BankingSystem.Features.InternetBank.Operator.AddUserDetails
{
    public class AddUserDetailsService
    {
        private readonly IAddUserDetailsRepository _repository;
        public AddUserDetailsService(IAddUserDetailsRepository repository)
        {
            _repository = repository;
        }
        public async Task<AddAccountResponse> AddAccountAsync(AddAccountRequest request)
        {
            var response = new AddAccountResponse();
            try
            {
                var newAccount = new AccountEntity();
                newAccount.UserId = request.UserId;

                IIbanValidator validator = new IbanValidator();
                ValidationResult validationResult = validator.Validate(request.IBAN);
                var accountExists = await _repository.AccountExists(request.IBAN);

                if (validationResult.IsValid && accountExists!=true)
                {
                    newAccount.IBAN = request.IBAN;
                    newAccount.Currency = request.Currency;
                    newAccount.Balance = request.Amount;
                    await _repository.AddAccountAsync(newAccount);
                    await _repository.SaveChangesAsync();
                    response.IsSuccessful = true;
                    response.AccountId = newAccount.Id;
                } else
                {
                    response.IsSuccessful = false;
                    response.ErrorMessage = "Invalid IBAN or Account already exists";
                }
                return response;
            }
            catch(Exception ex) 
            {
                response.IsSuccessful = false;
                response.ErrorMessage =  ex.Message;
                return response;
            }
        }

        public async Task<AddCardResponse> AddCardAsync(AddCardRequest request)
        {
            var response = new AddCardResponse();
            try
            {
                var cardByCardNumber = await _repository.CardtExists(request.CardNumber);
                if(cardByCardNumber == true)
                {
                    response.IsSuccessful = false;
                    response.ErrorMessage = "Card with this number already exists";
                }
                else
                {
                    var newCard = new CardEntity();
                    newCard.AccountId = request.AccountId;
                    newCard.FullName = request.FullName;
                    newCard.CardNumber = request.CardNumber;
                    newCard.ExpirationDate = request.ExpirationDate;
                    newCard.CVV = request.CVV;
                    newCard.PIN = request.PIN;

                    await _repository.AddCardAsync(newCard);
                    await _repository.SaveChangesAsync();

                    response.IsSuccessful = true;
                    response.CardId = newCard.Id;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccessful = false;
                response.ErrorMessage = ex.Message;
            }
            return response;
        }
    }
}
