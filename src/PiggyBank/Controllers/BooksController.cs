using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;

using PiggyBank.Models;
using PiggyBank.Models.Data;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace PiggyBank.Controllers
{
    [Route("api/users/{userId}/[controller]")]
    public class BooksController : Controller
    {
        [FromServices]
        public IPiggyBankRepository Repo { get; set; }

        [HttpGet]
        public IActionResult List(int userId, [FromHeader] string authorization)
        {
            try
            {
                User user = GetUser(userId, authorization);
                return new ObjectResult(user.Books);
            }
            catch (PiggyBankUserException e) { return HttpUnauthorized(); }
            catch (PiggyBankDataNotFoundException e) { return HttpUnauthorized(); }
        }

        [HttpPost]
        public IActionResult Post(int userId, [FromBody] Book book, [FromHeader] string authorization)
        {
            try
            {
                User user = GetUser(userId, authorization);
                Book bookCreated = Repo.BookManager.CreateBook(user, book);
                return CreatedAtRoute("GetBook", new { controller = "books", userId = userId, bookId = bookCreated.Id }, bookCreated);
            }
            catch (PiggyBankUserException e) { return HttpUnauthorized(); }
            catch (PiggyBankDataNotFoundException e) { return HttpUnauthorized(); }
            catch (PiggyBankDataException e) { return HttpBadRequest(new { error = e.Message }); }
        }

        [HttpGet("{bookId}", Name ="GetBook")]
        public IActionResult Get(int userId, int bookId, [FromHeader] string authorization)
        {
            try
            {
                User user = GetUser(userId, authorization);
                Book book = Repo.BookManager.FindBook(bookId);
                if (book.User.Id != userId) return HttpUnauthorized();
                return new ObjectResult(book);
            }
            catch (PiggyBankUserException e) { return HttpUnauthorized(); }
            catch (PiggyBankDataNotFoundException e) { return HttpUnauthorized(); }
            catch (PiggyBankDataException e) { return HttpBadRequest(new { error = e.Message }); }
        }

        [HttpPut("{bookId}")]
        public IActionResult Put(int userId, int bookId, [FromBody] Book book, [FromHeader] string authorization)
        {
            try
            {
                if (book == null) return HttpBadRequest(new { error = "Book object not provided" });
                if (book.Id != bookId) return HttpBadRequest(new { error = "Invalid Book.Id [" + book.Id + "]" });
                User user = GetUser(userId, authorization);
                Book bookToUpdate = Repo.BookManager.FindBook(book.Id);
                if (bookToUpdate.User.Id != userId) return HttpUnauthorized();
                Repo.BookManager.UpdateBook(book);
                return new NoContentResult();
            }
            catch (PiggyBankUserException e) { return HttpUnauthorized(); }
            catch (PiggyBankDataNotFoundException e) { return HttpUnauthorized(); }
            catch (PiggyBankDataException e) { return HttpBadRequest(new { error = e.Message }); }
        }

        private User GetUser(int userId, string authorization)
        {
            User user = TokenRequirement.Fulfill(Repo, userId, authorization);
            return user;
        }
    }
}
