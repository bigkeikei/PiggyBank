using Moq;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

using PiggyBank.Models;
using System.Threading.Tasks;
using System.Data.Entity.Infrastructure;

namespace PiggyBank.UnitTesting.Mocks
{
    public class MockPiggyBankDbContext : IPiggyBankDbContext
    {
        public MockPiggyBankDbContext()
        {
            Data = new MockData();
            Init();
        }

        public MockPiggyBankDbContext(MockData data)
        {
            Data = data;
            Init();
        }

        private void Init()
        {
            Users = GetMockDbSet(Data.Users).Object;
            Books = GetMockDbSet(Data.Books).Object;
            Accounts = GetMockDbSet(Data.Accounts).Object;
            Transactions = GetMockDbSet(Data.Transactions).Object;
            SaveCount = 0;
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public int SaveChanges() { SaveCount++; return 1; }
        public Task<int> SaveChangesAsync() { return Task.FromResult(SaveChanges()); }
        public MockData Data { get; }
        public int SaveCount { get; private set; }

        private Mock<DbSet<T>> GetMockDbSet<T>(List<T> entities) where T : class
        {
            var mockSet = new Mock<DbSet<T>>();

            mockSet.As<IDbAsyncEnumerable<T>>()
                .Setup(m => m.GetAsyncEnumerator())
                .Returns(new MockDbAsyncEnumerator<T>(entities.GetEnumerator()));

            mockSet.As<IQueryable<T>>()
                .Setup(m => m.Provider)
                .Returns(new MockDbAsyncQueryProvider<T>(entities.AsQueryable().Provider));

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
            mockSet.Setup(m => m.FindAsync(It.IsAny<object[]>())).Returns((object[] arg) =>
            {
                if (arg == null || arg.Length != 1) return null;
                var prop = typeof(T).GetProperties().FirstOrDefault(p => p.Name.ToLower() == "id");
                if (prop == null || prop.PropertyType != typeof(int)) return null;
                int id = (int)arg[0];
                return Task.FromResult(entities.FirstOrDefault(b => (int)prop.GetValue(b) == id));
            });
            return mockSet;
        }
    }
}
