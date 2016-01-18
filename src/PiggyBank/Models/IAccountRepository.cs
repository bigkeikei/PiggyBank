using System.Collections.Generic;


namespace PiggyBank.Models
{
    public interface IAccountRepository
    {
        Account Create(Account account);
        Account Find(int Id);
        Account Update(Account account);
        IEnumerable<Account> List();
    }
}
