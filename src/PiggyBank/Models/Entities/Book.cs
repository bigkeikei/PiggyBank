using Newtonsoft.Json;
using System.Collections.Generic;

using System.ComponentModel;
using System.Linq;

namespace PiggyBank.Models
{
    public class Book
    {
        [PiggyBankIgnore]
        public int Id { get; set; }

        [PiggyBankMandatory]
        public string Name { get; set; }

        [DefaultValue(true)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool IsValid { get; set; }

        [PiggyBankMandatory]
        public string Currency { get; set; }

        [JsonIgnore]
        [PiggyBankIgnore]
        [PiggyBankMandatory]
        public virtual User User { get; set; }

        [JsonIgnore]
        [PiggyBankIgnore]
        public virtual ICollection<Account> Accounts { get; set; }

        [JsonIgnore]
        [PiggyBankIgnore]
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
