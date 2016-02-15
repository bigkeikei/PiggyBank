using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace PiggyBank.Models
{
    public class User
    {
        [PiggyBankIgnoreWhenUpdate]
        public int Id { get; set; }

        [PiggyBankMandatory]
        [PiggyBankIgnoreWhenUpdate]
        public string Name { get; set; }

        [PiggyBankMandatory]
        public string Email { get; set; }

        [DefaultValue(true)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool IsActive { get; set; }

        [JsonIgnore]
        [PiggyBankIgnoreWhenUpdate]
        public virtual UserAuthentication Authentication { get; set; }

        [JsonIgnore]
        [PiggyBankIgnoreWhenUpdate]
        public virtual ICollection<Book> Books { get; set; }
    }
}
