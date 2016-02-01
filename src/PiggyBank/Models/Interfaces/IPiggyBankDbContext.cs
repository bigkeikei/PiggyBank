
using System.Data.Entity;

namespace PiggyBank.Models
{
    public interface IPiggyBankDbContext
    {
        DbSet<User> Users { get; set; }
        DbSet<Book> Books { get; set; }
        DbSet<Account> Accounts { get; set; }
        DbSet<Transaction> Transactions { get; set; }

        int SaveChanges();
    }
}