using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace PiggyBank.Models
{
    public class Tag
    {
        [PiggyBankIgnoreWhenUpdate]
        public int Id { get; set; }

        [PiggyBankMandatory]
        public string Name { get; set; }

        [DefaultValue(true)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool IsActive { get; set; }

        [JsonIgnore]
        [PiggyBankIgnoreWhenUpdate]
        public virtual Book Book { get; set; }

        [JsonIgnore]
        [PiggyBankIgnoreWhenUpdate]
        public virtual ICollection<Transaction> Transacitons { get; set; }

    }
}
