using Newtonsoft.Json;
using System;
using System.ComponentModel;

using PiggyBank.Models.Data;

namespace PiggyBank.Models
{
    public class Transaction
    {
        [PiggyBankEFIgnore]
        public int Id { get; set; }

        [PiggyBankEFMandatory]
        public virtual Account DebitAccount { get; set; }

        [PiggyBankEFMandatory]
        public virtual Account CreditAccount { get; set; }

        [PiggyBankEFMandatory]
        public string Currency { get; set; }

        [PiggyBankEFMandatory]
        public double Amount { get; set; }

        [PiggyBankEFMandatory]
        public double BookAmount { get; set; }

        [PiggyBankEFMandatory]
        public string Description { get; set; }

        [PiggyBankEFMandatory]
        public DateTime? TransactionDate { get; set; }

        [DefaultValue(true)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool IsValid { get; set; }

        [DefaultValue(false)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool IsClosed { get; set; }

        [PiggyBankEFMandatory]
        [PiggyBankEFIgnore]
        [JsonIgnore]
        public virtual Book Book { get; set; }
    }
}
