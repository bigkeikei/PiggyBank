using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace PiggyBank.Models
{
    public class AccountDetail
    {
        [JsonIgnore]
        public Account Account { get; private set; }

        [JsonIgnore]
        public IEnumerable<Transaction> Transactions { get; private set; }

        public double Balance
        {
            get
            {
                double debitAmount = Transactions.Where(b =>
                    b.DebitAccount.Id == Account.Id).Sum(o => (double?)o.Amount) ?? 0;
                double creditAmount = Transactions.Where(b =>
                    b.CreditAccount.Id == Account.Id).Sum(o => (double?)o.Amount) ?? 0;
                return (debitAmount - creditAmount) * Account.DebitSign;
            }
        }

        public double BookBalance
        {
            get
            {
                double debitAmount = Transactions.Where(b =>
                    b.DebitAccount.Id == Account.Id).Sum(o => (double?)o.BookAmount) ?? 0;
                double creditAmount = Transactions.Where(b =>
                    b.CreditAccount.Id == Account.Id).Sum(o => (double?)o.BookAmount) ?? 0;
                return (debitAmount - creditAmount) * Account.DebitSign;
            }
        }

        public AccountDetail(Account account, IEnumerable<Transaction> transactions)
        {
            Account = account;
            Transactions = transactions;
        }
    }
}