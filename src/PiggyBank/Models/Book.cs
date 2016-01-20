using Newtonsoft.Json;
using System.Collections.Generic;

using PiggyBank.Models.Data;
using System.ComponentModel;

namespace PiggyBank.Models
{
    public class Book
    {
        public Book()
        {
            Accounts = new List<Account>();
        }
        [PiggyBankEFIgnore]
        public int Id { get; set; }

        public string Name { get; set; }

        [DefaultValue(true)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool IsValid { get; set; }

        public string Currency { get; set; }

        [JsonIgnore]
        [PiggyBankEFIgnore]
        public virtual User User { get; set; }

        [JsonIgnore]
        [PiggyBankEFIgnore]
        public virtual ICollection<Account> Accounts { get; set; }
    }
}
