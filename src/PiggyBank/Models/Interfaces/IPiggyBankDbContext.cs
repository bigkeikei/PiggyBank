
using System.Data.Entity;
using System.Threading.Tasks;

namespace PiggyBank.Models
{
    public interface IPiggyBankDbContext
    {
        DbSet<Book> Books { get; set; }
        DbSet<Account> Accounts { get; set; }
        DbSet<Transaction> Transactions { get; set; }

        int SaveChanges();
        Task<int> SaveChangesAsync();
    }
}