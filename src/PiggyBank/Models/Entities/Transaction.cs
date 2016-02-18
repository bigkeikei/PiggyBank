using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PiggyBank.Models
{
    public class Transaction
    {
        [PiggyBankIgnoreWhenUpdate]
        public int Id { get; set; }

        [PiggyBankMandatory]
        public virtual Account DebitAccount { get; set; }

        [PiggyBankMandatory]
        public virtual Account CreditAccount { get; set; }

        [PiggyBankMandatory]
        public string Currency { get; set; }

        [PiggyBankMandatory]
        public double Amount { get; set; }

        [PiggyBankMandatory]
        public double BookAmount { get; set; }

        [PiggyBankMandatory]
        public string Description { get; set; }

        [PiggyBankMandatory]
        public DateTime? TransactionDate { get; set; }

        [DefaultValue(true)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool IsValid { get; set; }

        [DefaultValue(false)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool IsClosed { get; set; }

        [PiggyBankMandatory]
        [ConcurrencyCheck]
        public DateTime? TimeStamp { get; set; }

        [PiggyBankMandatory]
        [PiggyBankIgnoreWhenUpdate]
        [JsonIgnore]
        public virtual Book Book { get; set; }
    }
}
