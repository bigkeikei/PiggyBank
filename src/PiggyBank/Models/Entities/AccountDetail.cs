using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace PiggyBank.Models
{
    public class AccountDetail
    {
        [JsonIgnore]
        public Account Account { get; private set; }

        public double Balance { get; private set; }

        public double BookBalance { get; private set; }

        public AccountDetail(Account account, double balance, double bookBalance)
        {
            Account = account;
            Balance = balance;
            BookBalance = bookBalance;
        }
    }
}