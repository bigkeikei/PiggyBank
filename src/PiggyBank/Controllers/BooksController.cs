using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;

using PiggyBank.Models;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace PiggyBank.Controllers
{
    [Route("api/users/{userId}/[controller]")]
    public class BooksController : Controller
    {
        [FromServices]
        public IPiggyBankRepository Repo { get; set; }

        [HttpGet]
        public async Task<IActionResult> List(int userId, [FromHeader] string authorization)
        {
            try
            {
                User user = await Repo.UserManager.FindUser(userId);
                return new ObjectResult(await Repo.BookManager.ListBooks(user));
            }
            catch (PiggyBankUserException) { return HttpUnauthorized(); }
            catch (PiggyBankDataNotFoundException) { return HttpUnauthorized(); }
        }

        [HttpPost]
        public async Task<IActionResult> Post(int userId, [FromBody] Book book, [FromHeader] string authorization)
        {
            try
            {
                User user = await Repo.UserManager.FindUser(userId);
                Book bookCreated = await Repo.BookManager.CreateBook(user, book);
                return CreatedAtRoute("GetBook", new { controller = "books", userId = userId, bookId = bookCreated.Id }, bookCreated);
            }
            catch (PiggyBankUserException) { return HttpUnauthorized(); }
            catch (PiggyBankDataNotFoundException) { return HttpUnauthorized(); }
            catch (PiggyBankDataException e) { return HttpBadRequest(new { error = e.Message }); }
        }

        [HttpGet("{bookId}", Name ="GetBook")]
        public async Task<IActionResult> Get(int userId, int bookId, [FromHeader] string authorization)
        {
            try
            {
                Book book = await Repo.BookManager.FindBook(bookId);
                if (book.User.Id != userId) return HttpUnauthorized();
                return new ObjectResult(book);
            }
            catch (PiggyBankUserException) { return HttpUnauthorized(); }
            catch (PiggyBankDataNotFoundException) { return HttpUnauthorized(); }
            catch (PiggyBankDataException e) { return HttpBadRequest(new { error = e.Message }); }
        }

        [HttpPut("{bookId}")]
        public async Task<IActionResult> Put(int userId, int bookId, [FromBody] Book book, [FromHeader] string authorization)
        {
            try
            {
                if (book == null) return HttpBadRequest(new { error = "Book object not provided" });
                if (book.Id != bookId) return HttpBadRequest(new { error = "Invalid Book.Id [" + book.Id + "]" });
                Book bookToUpdate = await Repo.BookManager.FindBook(book.Id);
                if (bookToUpdate.User.Id != userId) return HttpUnauthorized();
                await Repo.BookManager.UpdateBook(book);
                return new NoContentResult();
            }
            catch (PiggyBankUserException) { return HttpUnauthorized(); }
            catch (PiggyBankDataNotFoundException) { return HttpUnauthorized(); }
            catch (PiggyBankDataException e) { return HttpBadRequest(new { error = e.Message }); }
        }
    }
}
