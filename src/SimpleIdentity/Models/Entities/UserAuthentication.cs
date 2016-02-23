using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SimpleIdentity.Models
{
    public class UserAuthentication
    {
        [Key, ForeignKey("User")]
        public int Id { get; set; }

        public string Secret { get; set; }

        public string Nonce { get; set; }

        public virtual User User { get; set; }
    }
}
