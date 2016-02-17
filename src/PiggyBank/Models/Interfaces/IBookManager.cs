using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PiggyBank.Models
{
    public interface IBookManager
    {
        Task<IEnumerable<Book>> ListBooks(int userId);
        Task<Book> CreateBook(Book book);
        Task<Book> FindBook(int bookId);
        Task<Book> UpdateBook(Book book);
        Task CloseBook(int bookId, DateTime closingDate);
    }
}
