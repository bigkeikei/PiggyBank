using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace PiggyBank.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        [DefaultValue(true)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool IsActive { get; set; }

        [JsonIgnore]
        public virtual UserAuthentication Authentication { get; set; }

        [JsonIgnore]
        public virtual ICollection<UserAuthorization> Authorizations { get; set; }
    }
}
