using System;

namespace PiggyBank.Models.Data
{
    public class PiggyBankEFRepository : IPiggyBankRepository
    {
        public IUserManager UserManager { get; private set; }
        public IBookManager BookManager { get; private set; }
        public IAccountManager AccountManager { get; private set; }
        public ITransactionManager TransactionManager { get; private set; }
        
        public PiggyBankEFRepository(PiggyBankDbContext dbContext)
        {
            UserManager = new UserEFManager(dbContext);
            BookManager = new BookEFManager(dbContext);
            AccountManager = new AccountEFManager(dbContext);
            TransactionManager = new TransactionEFManager(dbContext);
        }

    }

    public class PiggyBankDataException : Exception
    {
        public PiggyBankDataException(string message) : base(message) { }
    }
}
