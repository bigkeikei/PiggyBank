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
        public virtual IEnumerable<Account> Accounts { get; set; }
        public virtual User User { get; set; }
    }
}
