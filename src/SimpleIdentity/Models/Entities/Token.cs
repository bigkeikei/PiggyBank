using System;
using Newtonsoft.Json;

namespace SimpleIdentity.Models
{
    public class Token
    {
        public int Id { get; set; }

        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

        public DateTime? TokenTimeout { get; set; }

        [JsonIgnore]
        public virtual User User { get; set; }

        [JsonIgnore]
        public virtual Client Client { get; set; }
    }
}
