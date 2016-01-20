using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace PiggyBank.Models
{
    public enum AccountType { Capital, Asset, Liability, Income, Expense}
    public class Account
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public AccountType Type { get; set; } 
        public bool IsValid { get; set; }
        public string Currency { get; set; }

        [JsonIgnore]
        public virtual Book Book { get; set; }
    }
}
