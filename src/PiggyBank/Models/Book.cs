using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PiggyBank.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsValid { get; set; }
        public string Currency { get; set; }

        [JsonIgnore]
        public virtual User User { get; set; }
        [JsonIgnore]
        public virtual ICollection<Account> Accounts { get; set; }
    }
}
