using Newtonsoft.Json;
using System;

namespace PiggyBank.Models.Data
{
    public class Transaction
    {
        [PiggyBankEFIgnore]
        public int Id { get; set; }

        public virtual Account DebitAccount { get; set; }

        public virtual Account CreditAccount { get; set; }

        public string Currency { get; set; }

        public double Amount { get; set; }

        public double BookAmount { get; set; }

        public string Description { get; set; }

        public DateTime? TransactionDate { get; set; }

        [PiggyBankEFIgnore]
        [JsonIgnore]
        public virtual Book Book { get; set; }
    }
}
