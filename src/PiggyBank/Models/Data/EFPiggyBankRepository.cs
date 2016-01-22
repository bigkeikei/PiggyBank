using System;

namespace PiggyBank.Models.Data
{
    public class EFPiggyBankRepository : IPiggyBankRepository
    {
        public IUserManager UserManager { get; private set; }
        public IBookManager BookManager { get; private set; }
        public IAccountManager AccountManager { get; private set; }
        public ITransactionManager TransactionManager { get; private set; }
        
        public EFPiggyBankRepository(PiggyBankDbContext dbContext)
        {
            UserManager = new EFUserManager(dbContext);
            BookManager = new EFBookManager(dbContext);
            AccountManager = new EFAccountManager(dbContext);
            TransactionManager = new EFTransactionManager(dbContext);
        }
    }
}
