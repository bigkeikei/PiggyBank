using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using PiggyBank.Models;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace PiggyBank.Controllers
{
    [Route("api")]
    public class BooksController : Controller
    {
        [FromServices]
        public IPiggyBankRepository Repo { get; set; }

        [HttpGet("users/{userId}/[controller]")]
        public IActionResult List(int userId, [FromHeader] string authorization)
        {
            User user = TokenRequirement.Fulfill(Repo, userId, authorization);
            if (user == null) return HttpUnauthorized();
            return new ObjectResult(user.Books);
        }

        [HttpPost("users/{userId}/[controller]")]
        public IActionResult Post(int userId, [FromBody] Book book, [FromHeader] string authorization)
        {
            User user = TokenRequirement.Fulfill(Repo, userId, authorization);
            if (user == null) return HttpUnauthorized();
            Book bookCreated = Repo.CreateBook(user, book);
            return CreatedAtRoute("GetBook", new { controller = "books", userId = userId, bookId = bookCreated.Id}, bookCreated);
        }

        [HttpGet("users/{userId}/[controller]/{bookId}", Name ="GetBook")]
        public IActionResult Get(int userId, int bookId, [FromHeader] string authorization)
        {
            User user = TokenRequirement.Fulfill(Repo, userId, authorization);
            if (user == null) return HttpUnauthorized();
            return new ObjectResult(Repo.FindBook(userId, bookId));
        }

        [HttpPut("users/{userId}/[controller]/{bookId}")]
        public IActionResult Put(int userId, [FromBody] Book book, [FromHeader] string authorization)
        {
            User user = TokenRequirement.Fulfill(Repo, userId, authorization);
            if (user == null) return HttpUnauthorized();
            Book bookToUpdate = Repo.FindBook(userId, book.Id);
            if (bookToUpdate == null) return HttpNotFound(new { error = "Book [" + book.Id + "] cannot be found in User [" + userId + "]" });
            Repo.UpdateBook(userId, book);
            return new NoContentResult();
        }
    }
}
