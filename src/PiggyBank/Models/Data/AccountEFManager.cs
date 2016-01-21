using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PiggyBank.Models.Data
{
    public class AccountEFManager :IAccountManager
    {
        private PiggyBankDbContext _dbContext;
        public AccountEFManager(PiggyBankDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Account CreateAccount(Book book, Account account)
        {
            if (book == null) throw new PiggyBankDataException("Book object is missing");
            if (account == null) throw new PiggyBankDataException("Account object is missing");
            account.Book = book;
            PiggyBankEFUtility.CheckMandatory(account);
            Account accountCreated = _dbContext.Accounts.Add(account);
            _dbContext.SaveChanges();
            return accountCreated;
        }

        public Account FindAccount(int accountId)
        {
            return _dbContext.Accounts.Find(accountId);
        }

        public Account UpdateAccount(Account account)
        {
            if (account == null) throw new PiggyBankDataException("Account object is missing");
            PiggyBankEFUtility.CheckMandatory(account);
            Account accountToUpdate = FindAccount(account.Id);
            if (accountToUpdate == null || !accountToUpdate.IsValid) throw new PiggyBankDataException("Account [" + account.Id + "] cannot be found");
            if (GetAccountDetail(accountToUpdate).Transactions.Any())
            {
                if (!account.IsValid) throw new PiggyBankDataException("Editing Account.IsValid is not supported for accounts with transactions");
                if (account.Type != accountToUpdate.Type) throw new PiggyBankDataException("Editing Account.Type is not supported for accounts with transactions");
                if (account.Currency != accountToUpdate.Currency) throw new PiggyBankDataException("Editing Account.Currency is not supported for accounts with transactions");
            }
            PiggyBankEFUtility.UpdateModel(accountToUpdate, account);
            _dbContext.SaveChanges();
            return accountToUpdate;
        }

        public AccountDetail GetAccountDetail(int accountId)
        {
            Account account = FindAccount(accountId);
            if (account == null || !account.IsValid) throw new PiggyBankDataException("Account [" + account.Id + "] cannot be found");
            return GetAccountDetail(account);
        }

        public AccountDetail GetAccountDetail(Account account)
        {
            return new AccountDetail(
                account, 
                _dbContext);
        }
    }
}
