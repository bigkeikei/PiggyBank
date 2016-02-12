using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;

namespace PiggyBank.Models
{
    public class BookManager : IBookManager
    {

        private IPiggyBankDbContext _dbContext;

        public BookManager(IPiggyBankDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Book>> ListBooks(User user)
        {
            var books = await (from b in _dbContext.Books
                         where b.User.Id == user.Id
                         select b).ToListAsync();
            return books;
        }

        public async Task<Book> CreateBook(User user, Book book)
        {
            if (user == null) throw new PiggyBankDataException("User object is missing");
            if (book == null) throw new PiggyBankDataException("Book object is missing");
            book.User = user;
            PiggyBankUtility.CheckMandatory(book);
            _dbContext.Books.Add(book);
            await _dbContext.SaveChangesAsync();
            return book;
        }

        public async Task<Book> FindBook(int bookId)
        {
            var q = await (from b in _dbContext.Books
                           where b.Id == bookId
                           select b).ToListAsync();
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
    }
}
