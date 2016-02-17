using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleIdentity.Models
{
    public class SimpleIdentityRepository : ISimpleIdentityRepository
    {
        public IUserManager UserManager { get; private set; }
        public SimpleIdentityRepository(ISimpleIdentityDbContext dbContext)
        {
            UserManager = new UserManager(dbContext);
        }
    }
}
