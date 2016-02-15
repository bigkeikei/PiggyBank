using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PiggyBank.Models
{
    public interface IBookManager
    {
        Task<IEnumerable<Book>> ListBooks(User user);
        Task<Book> CreateBook(User user, Book book);
        Task<Book> FindBook(int bookId);
        Task<Book> UpdateBook(Book book);
        Task CloseBook(int bookId, DateTime closingDate);
    }
}
