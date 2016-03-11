using Newtonsoft.Json;
using System.Collections.Generic;

using System.ComponentModel;
using System.Linq;

namespace PiggyBank.Models
{
    public class Book
    {
        [PiggyBankIgnoreWhenUpdate]
        public int Id { get; set; }

        [PiggyBankMandatory]
        public string Name { get; set; }

        [DefaultValue(true)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool IsValid { get; set; }

        [PiggyBankMandatory]
        public string Currency { get; set; }

        [JsonIgnore]
        [PiggyBankIgnoreWhenUpdate]
        [PiggyBankMandatory]
        public int UserId { get; set; }

        [JsonIgnore]
        [PiggyBankIgnoreWhenUpdate]
        public virtual ICollection<Account> Accounts { get; set; }

        [JsonIgnore]
        [PiggyBankIgnoreWhenUpdate]
        public virtual ICollection<Transaction> Transactions { get; set; }

        [JsonIgnore]
        [PiggyBankIgnoreWhenUpdate]
        public virtual ICollection<Tag> Tags { get; set; }
    }
}
