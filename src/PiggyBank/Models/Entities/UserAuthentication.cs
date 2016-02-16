using Newtonsoft.Json;
using System;
using System.Text;
using System.Security.Cryptography;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PiggyBank.Models
{
    public class UserAuthentication
    {
        [JsonIgnore]
        [Key, ForeignKey("User")]
        public int Id { get; set; }

        [JsonIgnore]
        public string Secret { get; set; }

        [JsonIgnore]
        public string Challenge { get; set; }

        [JsonIgnore]
        public DateTime? ChallengeTimeout { get; set; }

        [JsonIgnore]
        public virtual User User { get; set; }

        // for unit testing purpose
        [JsonIgnore]
        [NotMapped]
        public string Signature
        {
            get
            {
                MD5 md5 = MD5.Create();
                return Convert.ToBase64String(md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(Challenge + Secret)));
            }
        }
    }
}
