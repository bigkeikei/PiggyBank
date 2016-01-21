using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PiggyBank.Models.Data;

namespace PiggyBank.Models
{
    public class AccountDetail
    {
        private PiggyBankDbContext _dbContext;

        [JsonIgnore]
        public IEnumerable<Transaction> Transactions
        {
            get
            {
                return _dbContext.Transactions.Where(b =>
                    b.Book.Id == Account.Book.Id &&
                    b.IsValid &&
                    (b.DebitAccount.Id == Account.Id || b.CreditAccount.Id == Account.Id));
            }
        }
        [JsonIgnore]
        public Account Account { get; private set; }

        public double Balance
        {
            get
            {
                double debitAmount = _dbContext.Transactions.Where(b =>
                    b.Book.Id == Account.Book.Id &&
                    b.IsValid &&
                    b.DebitAccount.Id == Account.Id).Sum(o => (double?)o.Amount) ?? 0;
                double creditAmount = _dbContext.Transactions.Where(b =>
                    b.Book.Id == Account.Book.Id &&
                    b.IsValid &&
                    b.CreditAccount.Id == Account.Id).Sum(o => (double?)o.Amount) ?? 0;
                return (debitAmount - creditAmount) * Account.DebitSign;
            }
        }
        public double BookBalance
        {
            get
            {
                double debitAmount = _dbContext.Transactions.Where(b =>
                    b.Book.Id == Account.Book.Id &&
                    b.IsValid &&
                    b.DebitAccount.Id == Account.Id).Sum(o => (double?)o.BookAmount) ?? 0;
                double creditAmount = _dbContext.Transactions.Where(b =>
                    b.Book.Id == Account.Book.Id &&
                    b.IsValid &&
                    b.CreditAccount.Id == Account.Id).Sum(o => (double?)o.BookAmount) ?? 0;
                return (debitAmount - creditAmount) * Account.DebitSign;
            }
        }

        public AccountDetail(Account account, PiggyBankDbContext dbContext)
        {
            Account = account;
            _dbContext = dbContext;
        }
    }
}
