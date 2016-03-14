using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PiggyBank.Models
{
    public interface ITransactionManager
    {
        Task<IEnumerable<Transaction>> ListTransactions(int bookId, DateTime? periodStart, DateTime? periodEnd, int? noOfRecords);
        Task<long> CountTransactions(int bookId, DateTime? periodStart, DateTime? periodEnd);
        Task<Transaction> CreateTransaction(Book book, Transaction transaction);
        Task<Transaction> FindTransaction(int transactionId, bool populateBook = false);
        Task<Transaction> UpdateTransaction(Transaction transaction);
        Task AddTag(int transactionId, Tag tag);
        Task RemoveTag(int transactionId, int tagId);
    }
}
