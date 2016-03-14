using System.Data.Entity;
using System.Threading.Tasks;

namespace SimpleIdentity.Models
{
    public interface ISimpleIdentityDbContext
    {
        DbSet<User> Users { get; set; }
        DbSet<Token> Tokens { get; set; }
        DbSet<Authorization> Authorizations { get; set; }
        DbSet<Client> Clients { get; set; }
        DbSet<UserNonce> UserNonces { get; set; }

        int SaveChanges();
        Task<int> SaveChangesAsync();
    }
}