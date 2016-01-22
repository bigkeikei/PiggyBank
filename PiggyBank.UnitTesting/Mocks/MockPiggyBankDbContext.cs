using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PiggyBank.Models;
namespace PiggyBank.UnitTesting.Mocks
{
    public class MockPiggyBankDbContext
    {
        public List<User> Users { get; set; }
        public List<Book> Books { get; set; }
        public List<Account> Accounts { get; set; }
        public List<Transaction> Transactions { get; set; }

        public MockPiggyBankDbContext()
        {
            Users = new List<User>();
            Books = new List<Book>();
            Accounts = new List<Account>();
            Transactions = new List<Transaction>();
        }
    }
}
