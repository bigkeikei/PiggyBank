using System;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PiggyBank.Models
{
    public interface IAccountManager
    {
        Task<IEnumerable<Account>> ListAccounts(Book book);
        Task<IEnumerable<Account>> ListAccounts(int userId);
        Task<Account> CreateAccount(Book book, Account account);
        Task<Account> FindAccount(int accountId, bool populateBook = false);
        Task<Account> UpdateAccount(Account account);
        Task<AccountDetail> GetAccountDetail(int accountId);
        Task<IEnumerable<Transaction>> GetTransactions(int accountId, DateTime? periodStart, DateTime? periodEnd, int? noOfRecords);
        Task<long> GetTransactionCount(int accountId, DateTime? periodStart, DateTime? periodEnd);
    }
}
