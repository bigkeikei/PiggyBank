using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PiggyBank.Models
{
    public interface IBookManager
    {
        Book CreateBook(User user, Book book);
        Book FindBook(int bookId);
        Book UpdateBook(Book book);
    }
}
