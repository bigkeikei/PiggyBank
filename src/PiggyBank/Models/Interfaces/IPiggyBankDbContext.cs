
using System.Data.Entity;
using System.Threading.Tasks;

namespace PiggyBank.Models
{
    public interface IPiggyBankDbContext
    {
        DbSet<User> Users { get; set; }
        DbSet<Book> Books { get; set; }
        DbSet<Account> Accounts { get; set; }
        DbSet<Transaction> Transactions { get; set; }
        DbSet<Token> Tokens { get; set; }

        int SaveChanges();
        Task<int> SaveChangesAsync();
    }
}