using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PiggyBank.Models.Data;
using System.ComponentModel;

namespace PiggyBank.Models
{
    public enum AccountType { Capital, Asset, Liability, Income, Expense}
    public class Account
    {
        [PiggyBankEFIgnore]
        public int Id { get; set; }

        public string Name { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public AccountType Type { get; set; }

        [DefaultValue(true)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool IsValid { get; set; }

        public string Currency { get; set; }

        [JsonIgnore]
        [PiggyBankEFIgnore]
        public virtual Book Book { get; set; }
    }
}
