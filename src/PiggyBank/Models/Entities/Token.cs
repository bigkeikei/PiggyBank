using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PiggyBank.Models
{
    public class Token
    {
        public enum TokenResourceType { User, Book, Account }

        [Flags]
        public enum TokenScopes { Readable= 1, Editable = 2, Full = Readable | Editable }

        public int Id { get; set; }

        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

        public DateTime? TokenTimeout { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public TokenResourceType ResourceType { get; set; }

        public int ResourceId { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public TokenScopes Scopes { get; set; }

        [JsonIgnore]
        public virtual User User { get; set; }
    }
}
