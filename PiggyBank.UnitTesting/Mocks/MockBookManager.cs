using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PiggyBank.Models;

namespace PiggyBank.UnitTesting.Mocks
{
    public class MockBookManager : IBookManager
    {
        private MockPiggyBankDbContext _dbContext;
        private int _bookId;

        public MockBookManager(MockPiggyBankDbContext dbContext)
        {
            _dbContext = dbContext;
            _bookId = 0;
        }

        public Book CreateBook(User user, Book book)
        {
            if (user == null) throw new PiggyBankDataException("User object is missing");
            if (book == null) throw new PiggyBankDataException("Book object is missing");
            book.User = user;
            book.Id = ++_bookId;
            PiggyBankUtility.CheckMandatory(book);
            user.Books.Add(book);
            _dbContext.Books.Add(book);
            return book;
        }

        public Book FindBook(int bookId)
        {
            var q = _dbContext.Books.Where(b => b.Id == bookId);
            if (!q.Any()) throw new PiggyBankDataNotFoundException("Book [" + bookId + "] cannot be found");
            return q.First();
        }

        public Book UpdateBook(Book book)
        {
            if (book == null) throw new PiggyBankDataException("Book object is missing");
            PiggyBankUtility.CheckMandatory(book);
            Book bookToUpdate = FindBook(book.Id);
            if (!book.IsValid) throw new PiggyBankDataNotFoundException("Book [" + book.Id + "] cannot be found");
            PiggyBankUtility.UpdateModel(bookToUpdate, book);
            return bookToUpdate;
        }
    }
}
