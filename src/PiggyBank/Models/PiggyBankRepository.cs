using System;

namespace PiggyBank.Models
{
    public class PiggyBankRepository : IPiggyBankRepository
    {
        public IUserManager UserManager { get; private set; }
        public IBookManager BookManager { get; private set; }
        public IAccountManager AccountManager { get; private set; }
        public ITransactionManager TransactionManager { get; private set; }
        
        public PiggyBankRepository(IPiggyBankDbContext dbContext)
        {
            UserManager = new UserManager(dbContext);
            BookManager = new BookManager(dbContext);
            AccountManager = new AccountManager(dbContext);
            TransactionManager = new TransactionManager(dbContext);
        }
    }
}
