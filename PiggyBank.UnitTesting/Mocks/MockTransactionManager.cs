using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PiggyBank.Models;
namespace PiggyBank.UnitTesting.Mocks
{
    public class MockTransactionManager:ITransactionManager
    {
        private MockPiggyBankDbContext _dbContext;
        private int _transactionId;

        public MockTransactionManager(MockPiggyBankDbContext dbContext)
        {
            _dbContext = dbContext;
            _transactionId = 0;
        }

        public Transaction CreateTransaction(Book book, Transaction transaction)
        {
            if (book == null) throw new PiggyBankDataException("Book object is missing");
            if (transaction == null) throw new PiggyBankDataException("Transaction object is missing");

            transaction.Book = book;
            transaction.Id = _transactionId++;
            PiggyBankUtility.CheckMandatory(transaction);
            book.Transactions.Add(transaction);
            _dbContext.Transactions.Add(transaction);
            return transaction;
        }

        public Transaction FindTransaction(int transactionId)
        {
            return _dbContext.Transactions.Where(b => b.Id == transactionId).First();
        }

        public Transaction UpdateTransaction(Transaction transaction)
        {
            if (transaction == null) throw new PiggyBankDataException("Transaction object is missing");
            Transaction transactionToUpdate = FindTransaction(transaction.Id);
            if (transactionToUpdate == null) throw new PiggyBankDataException("Transaction [" + transaction.Id + "] cannot be found");
            PiggyBankUtility.CheckMandatory(transaction);
            PiggyBankUtility.UpdateModel(transactionToUpdate, transaction);
            return transactionToUpdate;
        }
    }
}
