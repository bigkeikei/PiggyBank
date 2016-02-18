using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.ComponentModel;

namespace SimpleIdentity.Models
{
    public class Authorization
    {
        public enum AuthResourceType { User, Book, Account }

        [Flags]
        public enum AuthScopes { Readable = 1, Editable = 2, Full = Readable | Editable }

        public int Id { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public AuthResourceType ResourceType { get; set; }

        public int ResourceId { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public AuthScopes Scopes { get; set; }

        public DateTime? GrantDate { get; set; }

        [DefaultValue(false)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool IsRevoked { get; set; }

        public virtual User User { get; set; }
    }
}
