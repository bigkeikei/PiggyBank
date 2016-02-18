using System.Threading.Tasks;

namespace PiggyBank.Models
{
    public interface ITransactionManager
    {
        Task<Transaction> CreateTransaction(Book book, Transaction transaction);
        Task<Transaction> FindTransaction(int transactionId);
        Task<Transaction> UpdateTransaction(Transaction transaction);
    }
}
