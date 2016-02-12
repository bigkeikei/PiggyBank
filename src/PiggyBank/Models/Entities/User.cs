using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace PiggyBank.Models
{
    public class User
    {
        [PiggyBankIgnore]
        public int Id { get; set; }

        [PiggyBankMandatory]
        [PiggyBankIgnore]
        public string Name { get; set; }

        [PiggyBankMandatory]
        public string Email { get; set; }

        [DefaultValue(true)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool IsActive { get; set; }

        [JsonIgnore]
        [PiggyBankIgnore]
        public virtual UserAuthentication Authentication { get; set; }

        [JsonIgnore]
        [PiggyBankIgnore]
        public virtual ICollection<Book> Books { get; set; }
    }
}
