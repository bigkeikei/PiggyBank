using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Threading.Tasks;

namespace PiggyBank.Models
{
    public class AccountManager : IAccountManager
    {
        private IPiggyBankDbContext _dbContext;
        public AccountManager(IPiggyBankDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Account>> ListAccounts(Book book)
        {
            var accounts = await (from b in _dbContext.Accounts
                            where b.Book.Id == book.Id
                            select b).ToListAsync();
            return accounts;
        }

        public async Task<Account> CreateAccount(Book book, Account account)
        {
            if (book == null) throw new PiggyBankDataException("Book object is missing");
            if (account == null) throw new PiggyBankDataException("Account object is missing");
            account.Book = book;
            PiggyBankUtility.CheckMandatory(account);
            _dbContext.Accounts.Add(account);
            await _dbContext.SaveChangesAsync();
            return account;
        }

        public async Task<Account> FindAccount(int accountId)
        {
            Account account = await _dbContext.Accounts.FindAsync(accountId);
            var q = await (from b in _dbContext.Accounts
                           where b.Id == accountId
                           select b).ToListAsync();
            if (!q.Any()) throw new PiggyBankDataNotFoundException("Account [" + accountId + "] cannot be found");
            return q.First();
        }

        public async Task<Account> UpdateAccount(Account account)
        {
            if (account == null) throw new PiggyBankDataException("Account object is missing");
            PiggyBankUtility.CheckMandatory(account);
            Account accountToUpdate = await FindAccount(account.Id);
            if (!accountToUpdate.IsValid) throw new PiggyBankDataNotFoundException("Account [" + account.Id + "] cannot be found");

            if ((await GetTransactions(account).AnyAsync()))
            {
                if (!account.IsValid) throw new PiggyBankDataException("Editing Account.IsValid is not supported for accounts with transactions");
                if (account.Type != accountToUpdate.Type) throw new PiggyBankDataException("Editing Account.Type is not supported for accounts with transactions");
                if (account.Currency != accountToUpdate.Currency) throw new PiggyBankDataException("Editing Account.Currency is not supported for accounts with transactions");
            }
            PiggyBankUtility.UpdateModel(accountToUpdate, account);
            _dbContext.SaveChanges();
            return accountToUpdate;
        }

        public async Task<AccountDetail> GetAccountDetail(int accountId)
        {
            Account account = await FindAccount(accountId);
            if (!account.IsValid) throw new PiggyBankDataNotFoundException("Account [" + account.Id + "] cannot be found");

            double balance = await GetTransactions(account).SumAsync(
                b => (b.DebitAccount.Id == accountId ? 1 : -1) * account.DebitSign * b.Amount);
            double bookBalance = await GetTransactions(account).SumAsync(
                b => (b.DebitAccount.Id == accountId ? 1 : -1) * account.DebitSign * b.BookAmount);

            return new AccountDetail(account, balance, bookBalance);
        }

        private IQueryable<Transaction> GetTransactions(Account account)
        {
            return (from b in _dbContext.Transactions
                    where b.IsValid &&
                    b.Book.Id == account.Book.Id &&
                    (b.DebitAccount.Id == account.Id || b.CreditAccount.Id == account.Id)
                    select b);
        }
    }
}
