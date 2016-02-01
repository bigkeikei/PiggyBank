using Moq;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

using PiggyBank.Models;

namespace PiggyBank.UnitTesting.Mocks
{
    public class MockPiggyBankDbContext : IPiggyBankDbContext
    {
        public MockPiggyBankDbContext()
        {
            Data = new MockData();
            Users = GetMockDbSet(Data.Users).Object;
            Books = GetMockDbSet(Data.Books).Object;
            Accounts = GetMockDbSet(Data.Accounts).Object;
            Transactions = GetMockDbSet(Data.Transactions).Object;
        }

        public MockPiggyBankDbContext(MockData data)
        {
            Data = data;
            Users = GetMockDbSet(Data.Users).Object;
            Books = GetMockDbSet(Data.Books).Object;
            Accounts = GetMockDbSet(Data.Accounts).Object;
            Transactions = GetMockDbSet(Data.Transactions).Object;
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public int SaveChanges() { return 1; }

        public MockData Data { get; }

        private Mock<DbSet<T>> GetMockDbSet<T>(List<T> entities) where T : class
        {
            var mockSet = new Mock<DbSet<T>>();
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(entities.AsQueryable().Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(entities.AsQueryable().Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(entities.AsQueryable().ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(entities.AsQueryable().GetEnumerator());
            mockSet.Setup(m => m.Add(It.IsAny<T>())).Returns((T t) => {
                entities.Add(t);
                return t;
            });
            mockSet.Setup(m => m.Find(It.IsAny<object[]>())).Returns((object[] arg) =>
            {
                if (arg == null || arg.Length != 1) return null;
                var prop = typeof(T).GetProperties().FirstOrDefault(p => p.Name.ToLower() == "id");
                if (prop == null || prop.PropertyType != typeof(int)) return null;
                int id = (int)arg[0];
                return entities.FirstOrDefault(b => (int)prop.GetValue(b) == id);
            });
            return mockSet;
        }
    }
}
