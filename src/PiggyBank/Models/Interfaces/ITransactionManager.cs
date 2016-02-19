using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PiggyBank.Models
{
    public interface ITransactionManager
    {
        Task<IEnumerable<Transaction>> ListTransactions(int bookId, DateTime periodStart, DateTime periodEnd);
        Task<long> CountTransactions(int bookId, DateTime periodStart, DateTime periodEnd);
        Task<Transaction> CreateTransaction(Book book, Transaction transaction);
        Task<Transaction> FindTransaction(int transactionId);
        Task<Transaction> UpdateTransaction(Transaction transaction);
    }
}
