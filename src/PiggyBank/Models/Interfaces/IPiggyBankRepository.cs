using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PiggyBank.Models
{
    public interface IPiggyBankRepository
    {
        IUserManager UserManager { get;  }
        IBookManager BookManager { get;  }
        IAccountManager AccountManager { get; }
        ITransactionManager TransactionManager { get; }
    }
}
