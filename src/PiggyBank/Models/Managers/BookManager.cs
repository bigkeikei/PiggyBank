using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace PiggyBank.Models
{
    public class BookManager : IBookManager
    {

        private IPiggyBankDbContext _dbContext;

        public BookManager(IPiggyBankDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Book>> ListBooks(int userId)
        {
            var q = _dbContext.Books.Where(b => b.UserId == userId);
            return await q.ToListAsync();
        }

        public async Task<Book> CreateBook(Book book)
        {
            if (book == null) throw new PiggyBankDataException("Book object is missing");
            PiggyBankUtility.CheckMandatory(book);
            _dbContext.Books.Add(book);
            await _dbContext.SaveChangesAsync();
            return book;
        }

        public async Task<Book> FindBook(int bookId)
        {
            /*
            var q = await (from b in _dbContext.Books
                           where b.Id == bookId
                           select b).ToListAsync();
            */
            var q = await _dbContext.Books.Where(b => b.Id == bookId).ToListAsync();
            if (!q.Any()) throw new PiggyBankDataNotFoundException("Book [" + bookId + "] cannot be found");
            return q.First();
        }

        public async Task<Book> UpdateBook(Book book)
        {
            if (book == null) throw new PiggyBankDataException("Book object is missing");

            Book bookToUpdate = await FindBook(book.Id);
            if (!bookToUpdate.IsValid) throw new PiggyBankDataNotFoundException("Book [" + book.Id + "] cannot be found");
            PiggyBankUtility.CheckMandatory(book);
            PiggyBankUtility.UpdateModel(bookToUpdate, book);
            await _dbContext.SaveChangesAsync();
            return bookToUpdate;
        }

        public async Task CloseBook(int bookId, DateTime closingDate)
        {
            DateTime timestamp = DateTime.Now;
            /*
            var q = (from t in _dbContext.Transactions
                     where !t.IsClosed &&
                     t.IsValid &&
                     t.Book.Id == bookId &&
                     t.TransactionDate <= closingDate
                     select t);
            */
            var q = _dbContext.Transactions.Where(
                t => !t.IsClosed &&
                    t.IsValid &&
                    t.Book.Id == bookId &&
                    t.TransactionDate <= closingDate);
            List<Transaction> transactions = await q.ToListAsync();
            foreach(Transaction t in transactions)
            {
                t.IsClosed = true;
                UpdateAccountClosing(t, t.DebitAccount, 1, closingDate, timestamp);
                UpdateAccountClosing(t, t.CreditAccount, -1, closingDate, timestamp);
            }
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) { throw new PiggyBankDataConcurrencyException("Closing has already been run by another process"); }
        }

        private void UpdateAccountClosing(Transaction transaction, Account account, int debitSign, DateTime closingDate, DateTime timestamp)
        {
            if (account.Closing == null)
            {
                account.Closing = new AccountClosing();
                account.Closing.Account = account;
                account.Closing.Id = account.Id;
            }
            account.Closing.Amount += account.DebitSign * debitSign * transaction.Amount;
            account.Closing.BookAmount += account.DebitSign * debitSign * transaction.BookAmount;
            account.Closing.ClosingDate = closingDate;
            account.Closing.TimeStamp = timestamp;
        }
    }
}
