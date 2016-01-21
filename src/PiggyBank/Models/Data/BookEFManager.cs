using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PiggyBank.Models.Data
{
    public class BookEFManager : IBookManager
    {

        private PiggyBankDbContext _dbContext;

        public BookEFManager(PiggyBankDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Book CreateBook(User user, Book book)
        {
            if (user == null) throw new PiggyBankDataException("User object is missing");
            if (book == null) throw new PiggyBankDataException("Book object is missing");
            book.User = user;
            PiggyBankEFUtility.CheckMandatory(book);
            Book bookCreated = _dbContext.Books.Add(book);
            _dbContext.SaveChanges();
            return bookCreated;
        }

        public Book FindBook(int bookId)
        {
            return _dbContext.Books.Find(bookId);
        }

        public Book UpdateBook(Book book)
        {
            if (book == null) throw new PiggyBankDataException("Book object is missing");

            Book bookToUpdate = FindBook(book.Id);
            if (bookToUpdate == null) throw new PiggyBankDataException("Book [" + book.Id + "] cannot be found");
            PiggyBankEFUtility.CheckMandatory(book);
            PiggyBankEFUtility.UpdateModel(bookToUpdate, book);
            _dbContext.SaveChanges();
            return bookToUpdate;
        }
    }
}
