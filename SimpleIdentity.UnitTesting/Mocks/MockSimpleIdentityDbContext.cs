using System.Data.Entity;
using SimpleIdentity.Models;
using System.Threading.Tasks;

namespace SimpleIdentity.UnitTesting.Mocks
{
    public class MockSimpleIdentityDbContext : Mock.MockDbContext, ISimpleIdentityDbContext
    {
        public MockSimpleIdentityDbContext()
        {
            Data = new MockData();
            Init();
        }

        public MockSimpleIdentityDbContext(MockData data)
        {
            Data = data;
            Init();
        }

        private void Init()
        {
            Users = GetMockDbSet(Data.Users).Object;
            Tokens = GetMockDbSet(Data.Tokens).Object;
            Authorizations = GetMockDbSet(Data.Authorizations).Object;
            Clients = GetMockDbSet(Data.Clients).Object;

            SaveCount = 0;
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Token> Tokens { get; set; }
        public DbSet<Authorization> Authorizations { get; set; }
        public DbSet<Client> Clients { get; set; }

        public MockData Data { get; }
        public int SaveCount { get; private set; }

        public int SaveChanges() { SaveCount++; return 1; }
        public Task<int> SaveChangesAsync() { return Task.FromResult(SaveChanges()); }
    }
}
