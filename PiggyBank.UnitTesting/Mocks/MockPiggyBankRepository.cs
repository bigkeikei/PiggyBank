using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PiggyBank.Models;
using PiggyBank.Models.Data;

namespace PiggyBank.UnitTesting.Mocks
{
    public class MockPiggyBankRepository : IPiggyBankRepository
    {
        public IUserManager UserManager { get; private set; }
        public IBookManager BookManager { get; private set; }
        public IAccountManager AccountManager { get; private set; }
        public ITransactionManager TransactionManager { get; private set; }
        private MockPiggyBankDbContext _dbContext;

        public MockPiggyBankRepository(PiggyBankDbContext dbContext)
        {
            _dbContext = new MockPiggyBankDbContext();
            UserManager = new MockUserManager(_dbContext);
            BookManager = new MockBookManager(_dbContext);
            AccountManager = new MockAccountManager(_dbContext);
            TransactionManager = new MockTransactionManager(_dbContext);
        }
    }
}
