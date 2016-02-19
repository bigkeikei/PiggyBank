using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
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
            return await ListAccounts(b => b.Book.Id == book.Id);
        }

        public async Task<IEnumerable<Account>> ListAccounts(int userId)
        {
            return await ListAccounts(b => b.Book.UserId == userId);
        }

        private async Task<IEnumerable<Account>> ListAccounts(Expression<Func<Account, bool>> options)
        {
            var accounts = await _dbContext.Accounts
                .Where(b => b.IsValid)
                .Where(options)
                .ToListAsync();
            //if (!accounts.Any()) { throw new PiggyBankDataNotFoundException("Account cannot be found by expression " + options.ToString())}
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
            var q = await (from b in _dbContext.Accounts
                           where b.Id == accountId
                           select b).ToListAsync();
            if (!q.Any()) throw new PiggyBankDataNotFoundException("Account [" + accountId + "] cannot be found");
            return q.First();
        }

        public async Task<Account> FindAccount(int accountId, int userId)
        {
            var q = await (from b in _dbContext.Accounts
                           where b.Id == accountId &&
                           b.Book.UserId == userId
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
            return await GetAccountDetail(account);
        }

        private async Task<AccountDetail> GetAccountDetail(Account account)
        {
            if (!account.IsValid) throw new PiggyBankDataNotFoundException("Account [" + account.Id + "] cannot be found");

            double amount = 0, bookAmount = 0;
            int noOfTransactions = 0;

            if (account.Closing != null)
            {
                amount += account.Closing.Amount ?? 0;
                bookAmount += account.Closing.BookAmount ?? 0;
            }

            var q = await GetTransactions(account)
                .Select(b => new
                    {
                        Amount = (b.DebitAccount.Id == account.Id ? 1 : -1) * (b.IsClosed ? 0 : 1) * account.DebitSign * b.Amount,
                        BookAmount = (b.DebitAccount.Id == account.Id ? 1 : -1) * (b.IsClosed ? 0 : 1) * account.DebitSign * b.BookAmount,
                        Group = 1
                    })
                .GroupBy(b => b.Group)
                .Select(g => new
                    {
                        Amount = g.Sum(x => x.Amount),
                        BookAmount = g.Sum(x => x.BookAmount),
                        NoOfTransactions = g.Count()
                    })
                .ToListAsync();
            /*
            var q = await (from b in 
                               (from b in GetTransactions(account)
                                select new
                                {
                                    Amount = (b.DebitAccount.Id == account.Id ? 1 : -1) * (b.IsClosed ? 0 : 1) * account.DebitSign * b.Amount,
                                    BookAmount = (b.DebitAccount.Id == account.Id ? 1 : -1) * (b.IsClosed ? 0 : 1) * account.DebitSign * b.BookAmount,
                                    Group = 1
                                })
                           group b by b.Group into g
                           select new
                           {
                               Amount = g.Sum(x => x.Amount),
                               BookAmount = g.Sum(x => x.BookAmount),
                               NoOfTransactions = g.Count()
                           }).ToListAsync();
            */
            if (q.Any())
            {
                var result = q.First();
                amount += result.Amount;
                bookAmount += result.BookAmount;
                noOfTransactions += result.NoOfTransactions;
            }

            return new AccountDetail(account, amount, bookAmount, noOfTransactions);
        }

        public async Task<IEnumerable<Transaction>> GetTransactions(int accountId)
        {
            const int recordLimit = 100;
            Account account = await FindAccount(accountId);
            List<Transaction> transactions = new List<Transaction>();
            return await GetTransactions(account)
                .OrderByDescending(b => b.TransactionDate)
                .Take(recordLimit)
                .ToListAsync();
        }

        public async Task<long> GetTransactionCount(int accountId)
        {
            Account account = await FindAccount(accountId);
            List<Transaction> transactions = new List<Transaction>();
            return await GetTransactions(account)
                .LongCountAsync();
        }

        private IQueryable<Transaction> GetTransactions(Account account)
        {
            int bookId = account.Book.Id;
            return _dbContext.Transactions
                .Where(b => b.IsValid &&
                    b.Book.Id == bookId &&
                    (b.DebitAccount.Id == account.Id || b.CreditAccount.Id == account.Id));
        }
    }
}
