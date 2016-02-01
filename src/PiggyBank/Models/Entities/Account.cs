using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace PiggyBank.Models
{
    public enum AccountType { Capital, Asset, Liability, Income, Expense}
    public class Account
    {
        [PiggyBankIgnore]
        public int Id { get; set; }

        [PiggyBankMandatory]
        public string Name { get; set; }

        [PiggyBankMandatory]
        [JsonConverter(typeof(StringEnumConverter))]
        public AccountType Type { get; set; }

        [DefaultValue(true)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool IsValid { get; set; }

        [PiggyBankMandatory]
        public string Currency { get; set; }

        [JsonIgnore]
        [PiggyBankIgnore]
        public virtual Book Book { get; set; }

        [JsonIgnore]
        [PiggyBankIgnore]
        [NotMapped]
        public int DebitSign
        {
            get
            {
                switch (Type)
                {
                    case AccountType.Expense:
                    case AccountType.Asset:
                        return 1;
                    case AccountType.Capital:
                    case AccountType.Liability:
                    case AccountType.Income:
                        return -1;
                    default:
                        return 0;
                }
            }
        }
    }
}
