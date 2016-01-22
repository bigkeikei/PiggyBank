using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PiggyBank.Models;
namespace PiggyBank.UnitTesting.Mocks
{
    public class MockAccountManager : IAccountManager
    {
        private MockPiggyBankDbContext _dbContext;
        private int _accountId;

        public MockAccountManager(MockPiggyBankDbContext dbContext)
        {
            _dbContext = dbContext;
            _accountId = 0;
        }

        public Account CreateAccount(Book book, Account account)
        {
            if (book == null) throw new PiggyBankDataException("Book object is missing");
            if (account == null) throw new PiggyBankDataException("Account object is missing");
            account.Book = book;
            account.Id = _accountId++;
            PiggyBankUtility.CheckMandatory(account);
            book.Accounts.Add(account);
            _dbContext.Accounts.Add(account);
            return account;
        }

        public Account FindAccount(int accountId)
        {
            var q = _dbContext.Accounts.Where(b => b.Id == accountId);
            if (!q.Any()) throw new PiggyBankDataNotFoundException("Account [" + accountId + "] cannot be found");
            return q.First();
        }

        public Account UpdateAccount(Account account)
        {
            if (account == null) throw new PiggyBankDataException("Account object is missing");
            PiggyBankUtility.CheckMandatory(account);
            Account accountToUpdate = FindAccount(account.Id);
            if (!accountToUpdate.IsValid) throw new PiggyBankDataNotFoundException("Account [" + account.Id + "] cannot be found");
            PiggyBankUtility.UpdateModel(accountToUpdate, account);
            return accountToUpdate;
        }

        public AccountDetail GetAccountDetail(int accountId)
        {
            Account account = FindAccount(accountId);
            if (!account.IsValid) throw new PiggyBankDataException("Account [" + account.Id + "] cannot be found");
            return new AccountDetail(
                account,
                _dbContext.Transactions.Where(b =>
                    b.IsValid &&
                    b.Book.Id == account.Book.Id &&
                    (b.DebitAccount.Id == account.Id || b.CreditAccount.Id == account.Id)));
        }
    }
}
