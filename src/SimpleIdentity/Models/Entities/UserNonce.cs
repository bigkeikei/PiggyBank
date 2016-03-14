using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleIdentity.Models
{
    public class UserNonce
    {
        public int Id { get; set; }
        public string Nonce { get; set; }
        public DateTime? Timeout { get; set; }
        public bool IsValid { get; set; }

        public virtual User User { get; set; }
    }
}
