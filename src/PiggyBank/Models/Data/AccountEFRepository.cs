using System.Collections.Generic;

namespace PiggyBank.Models.Data
{
    public class AccountEFRepository : IAccountRepository
    {
        private PiggyBankDbContext _dbContext;

        public AccountEFRepository(PiggyBankDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Account Create(Account account)
        {
            Account accountCreated = _dbContext.Accounts.Add(account);
            _dbContext.SaveChanges();
            return accountCreated;
        }

        public Account Find(int id)
        {
            return _dbContext.Accounts.Find(id);
        }

        public Account Update(Account account)
        {
            var accountToUpdate = _dbContext.Accounts.Find(account.Id);
            accountToUpdate.IsValid = account.IsValid;
            accountToUpdate.Name = account.Name;
            accountToUpdate.Type = account.Type;
            _dbContext.SaveChanges();
            return accountToUpdate;
        }

        public IEnumerable<Account> List()
        {
            return _dbContext.Accounts;
        }
    }
}
