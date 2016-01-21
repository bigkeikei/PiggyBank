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
            if (book == null || account == null) return null;
            account.Book = book;
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
            if (account == null) return null;
            Account accountToUpdate = FindAccount(account.Id);
            if (accountToUpdate == null) throw new PiggyBankDataException("Account [" + account.Id + "] cannot be found");
            PiggyBankEFUtility.UpdateModel(accountToUpdate, account);
            _dbContext.SaveChanges();
            return accountToUpdate;
        }
    }
}
