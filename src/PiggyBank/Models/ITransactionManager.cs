using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PiggyBank.Models
{
    public interface ITransactionManager
    {
        Transaction CreateTransaction(Book book, Transaction transaction);
        Transaction FindTransaction(int transactionId);
        Transaction UpdateTransaction(Transaction transaction);
    }
}
