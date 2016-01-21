using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PiggyBank.Models
{
    public interface IAccountManager
    {
        Account CreateAccount(Book book, Account account);
        Account FindAccount(int accountId);
        Account UpdateAccount(Account account);
    }
}
