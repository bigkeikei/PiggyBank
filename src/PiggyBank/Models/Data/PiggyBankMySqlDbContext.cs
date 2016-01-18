using System.Data.Entity;
using MySql.Data.Entity;

namespace PiggyBank.Models.Data
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class PiggyBankMySqlDbContext : PiggyBankDbContext
    {
        public PiggyBankMySqlDbContext(string connectionString) : base(connectionString)
        {
        }
    }
}
