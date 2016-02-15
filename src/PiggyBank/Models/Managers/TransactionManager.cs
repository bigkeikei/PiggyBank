using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;

namespace PiggyBank.Models
{
    public class TransactionManager : ITransactionManager
    {
        private IPiggyBankDbContext _dbContext;

        public TransactionManager(IPiggyBankDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Transaction> CreateTransaction(Book book, Transaction transaction)
        {
            DateTime timeStamp = DateTime.Now;

            if (book == null) throw new PiggyBankDataException("Book object is missing");
            if (transaction == null) throw new PiggyBankDataException("Transaction object is missing");

            transaction.Book = book;
            AccountManager acc = new AccountManager(_dbContext);
            transaction.DebitAccount = await acc.FindAccount(transaction.DebitAccount.Id);
            transaction.CreditAccount = await acc.FindAccount(transaction.CreditAccount.Id);

            // DR/CR account validation
            if (transaction.DebitAccount == null || !transaction.DebitAccount.IsValid) throw new PiggyBankDataException("Invalid Debit Account[" + transaction.DebitAccount.Id + "]");
            if (transaction.CreditAccount == null || !transaction.CreditAccount.IsValid) throw new PiggyBankDataException("Invalid Credit Account[" + transaction.CreditAccount.Id + "]");

            // Book validation
            if (transaction.DebitAccount.Book.Id != book.Id) throw new PiggyBankDataException("Debit Account[" + transaction.DebitAccount.Id + "] cannot be found in Book [" + book.Id +"]");
            if (transaction.CreditAccount.Book.Id != book.Id) throw new PiggyBankDataException("Credit Account[" + transaction.CreditAccount.Id + "] cannot be found in Book [" + book.Id + "]");
            
            // Currency validation
            if (transaction.DebitAccount.Currency != transaction.Currency && transaction.DebitAccount.Currency != book.Currency) throw new PiggyBankDataException("Invalid DebitAccount.Currency[" + transaction.DebitAccount.Currency + "]");
            if (transaction.CreditAccount.Currency != transaction.Currency && transaction.CreditAccount.Currency != book.Currency) throw new PiggyBankDataException("Invalid CreditAccount.Currency[" + transaction.CreditAccount.Currency + "]");

            // Book amount validation
            if (transaction.BookAmount <= 0) throw new PiggyBankDataException("Invalid Transaction.BookAmount [" + transaction.BookAmount + "]");

            // Amount validation
            if (transaction.BookAmount < 0) throw new PiggyBankDataException("Invalid Transaction.Amount [" + transaction.Amount + "]");

            transaction.TimeStamp = timeStamp;

            PiggyBankUtility.CheckMandatory(transaction);
            _dbContext.Transactions.Add(transaction);
            await _dbContext.SaveChangesAsync();

            return transaction;
        }

        public async Task<Transaction> FindTransaction(int transactionId)
        {
            Transaction transaction = await _dbContext.Transactions.FindAsync(transactionId);
            var q = await (from b in _dbContext.Transactions
                           where b.Id == transactionId
                           select b).ToListAsync();
            if (!q.Any()) throw new PiggyBankDataNotFoundException("Transaction [" + transactionId + "] cannot be found");
            return q.First();
        }

        public async Task<Transaction> UpdateTransaction(Transaction transaction)
        {
            DateTime timeStamp = DateTime.Now;
            if (transaction == null) throw new PiggyBankDataException("Transaction object is missing");

            Transaction transactionToUpdate = await FindTransaction(transaction.Id);
            if (!transactionToUpdate.IsValid) throw new PiggyBankDataNotFoundException("Transaction [" + transaction.Id + "] cannot be found");
            if (transaction.TimeStamp != transactionToUpdate.TimeStamp) throw new PiggyBankDataConcurrencyException("Transaction [" + transaction.Id + "] is being updated by other process");
            transaction.TimeStamp = timeStamp;
            PiggyBankUtility.CheckMandatory(transaction);
            PiggyBankUtility.UpdateModel(transactionToUpdate, transaction);
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) { throw new PiggyBankDataConcurrencyException("Transaction [" + transaction.Id + "] is being updated by other process"); }

            return transactionToUpdate;
        }
    }
}
