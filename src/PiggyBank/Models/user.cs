using System.Collections.Generic;
using Newtonsoft.Json;

namespace PiggyBank.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        [JsonIgnore]
        public string Secret { get; set; }
        [JsonIgnore]
        public virtual UserAuthentication Authentication { get; set; }
        [JsonIgnore]
        public virtual IEnumerable<Book> Books { get; set; }
    }
}
