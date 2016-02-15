using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PiggyBank.Models
{
    public class AccountClosing
    {
        public AccountClosing()
        {
            Amount = 0;
            BookAmount = 0;
        }

        [JsonIgnore]
        [Key, ForeignKey("Account")]
        public int Id { get; set; }

        [PiggyBankMandatory]
        public DateTime? ClosingDate { get; set; }

        public double? Amount { get; set; }

        public double? BookAmount { get; set; }

        [ConcurrencyCheck]
        public DateTime? TimeStamp { get; set; }

        [JsonIgnore]
        public virtual Account Account { get; set; }
    }
}
