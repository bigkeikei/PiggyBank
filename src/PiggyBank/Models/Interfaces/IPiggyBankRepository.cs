using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PiggyBank.Models
{
    public interface IPiggyBankRepository
    {
        IBookManager BookManager { get;  }
        IAccountManager AccountManager { get; }
        ITransactionManager TransactionManager { get; }
        ITagManager TagManager { get; }
    }
}
