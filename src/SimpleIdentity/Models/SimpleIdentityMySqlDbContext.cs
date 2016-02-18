using System.Data.Entity;
using MySql.Data.Entity;

namespace SimpleIdentity.Models
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class SimpleIdentityMySqlDbContext : DbContext, ISimpleIdentityDbContext
    {
        public SimpleIdentityMySqlDbContext(string connectionString) : base(connectionString)
        {
            Database.CreateIfNotExists();
        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Token> Tokens { get; set; }
        public virtual DbSet<Authorization> Authorizations { get; set; }
        public virtual DbSet<Client> Clients { get; set; }
    }
}
