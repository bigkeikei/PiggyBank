using System.Collections.Generic;

namespace PiggyBank.Models
{
    public interface IPiggyBankRepository
    {
        IUserManager UserManager { get; }
        IBookManager BookManager { get; }
        IAccountManager AccountManager { get; }
    }
}
