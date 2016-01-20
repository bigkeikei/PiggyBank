using System.Collections.Generic;
using Newtonsoft.Json;

using PiggyBank.Models.Data;
using System.ComponentModel;

namespace PiggyBank.Models
{
    public class User
    {
        public User()
        {
            Books = new List<Book>();
        }

        [PiggyBankEFIgnore]
        public int Id { get; set; }

        [PiggyBankEFIgnore]
        public string Name { get; set; }

        public string Email { get; set; }

        [DefaultValue(true)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool IsActive { get; set; }

        [JsonIgnore]
        [PiggyBankEFIgnore]
        public virtual UserAuthentication Authentication { get; set; }

        [JsonIgnore]
        [PiggyBankEFIgnore]
        public virtual ICollection<Book> Books { get; set; }
    }
}
