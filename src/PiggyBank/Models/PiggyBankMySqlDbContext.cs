using System.Data.Entity;
using MySql.Data.Entity;

namespace PiggyBank.Models
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class PiggyBankMySqlDbContext : DbContext, IPiggyBankDbContext
    {
        public PiggyBankMySqlDbContext(string connectionString) : base(connectionString)
        {
            Database.CreateIfNotExists();
        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Book> Books { get; set; }
        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<Transaction> Transactions { get; set; }
        public virtual DbSet<Token> Tokens { get; set; }
    }
}
