using System.Collections.Generic;
using System.Threading.Tasks;

namespace PiggyBank.Models
{
    public interface IAccountManager
    {
        Task<IEnumerable<Account>> ListAccounts(Book book);
        Task<Account> CreateAccount(Book book, Account account);
        Task<Account> FindAccount(int accountId);
        Task<Account> UpdateAccount(Account account);
        Task<AccountDetail> GetAccountDetail(int accountId);
    }
}
