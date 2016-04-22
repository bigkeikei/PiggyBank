using System.Data.Entity;
using MySql.Data.Entity;
using System.Diagnostics;

namespace PiggyBank.Models
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class PiggyBankMySqlDbContext : DbContext, IPiggyBankDbContext
    {
        public PiggyBankMySqlDbContext(string connectionString) : base(connectionString)
        {
            //this.Database.Log = (s => Debug.WriteLine(s));
            Database.CreateIfNotExists();
        }

        public virtual DbSet<Book> Books { get; set; }
        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<Transaction> Transactions { get; set; }
        public virtual DbSet<Tag> Tags { get; set; }
    }
}
