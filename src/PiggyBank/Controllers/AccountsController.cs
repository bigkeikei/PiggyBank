using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;

using PiggyBank.Controllers.Exceptions;
using PiggyBank.Models;
using PiggyBank.Models.Data;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace PiggyBank.Controllers
{
    [Route("api/users/{userId}/books/{bookId}/[controller]")]
    public class AccountsController : Controller
    {
        [FromServices]
        public IPiggyBankRepository Repo { get; set; }

        [HttpGet]
        public IActionResult List(int userId, int bookId, [FromHeader] string authorization)
        {
            try
            {
                return new ObjectResult(GetBook(userId, bookId, authorization).Accounts);
            }
            catch (PiggyBankUserException e) { return HttpUnauthorized(); }
            catch (PiggyBankBookException e) { return HttpNotFound(new { error = e.Message }); }
        }

        [HttpPost]
        public IActionResult Post(int userId, int bookId, [FromBody] Account account, [FromHeader] string authorization)
        {
            try
            {
                Book book = GetBook(userId, bookId, authorization);
                Account accountCreated = Repo.CreateAccount(book, account);
                return CreatedAtRoute("GetAccount", new { controller = "accounts", userId = userId, bookId = bookId, accountId = accountCreated.Id }, accountCreated);
            }
            catch (PiggyBankUserException e) { return HttpUnauthorized(); }
            catch (PiggyBankBookException e) { return HttpNotFound(new { error = e.Message }); }
            catch (PiggyBankDataException e) { return HttpBadRequest(new { error = e.Message }); }
        }

        [HttpGet("{accountId}", Name = "GetAccount")]
        public IActionResult Get(int userId, int bookId, int accountId, [FromHeader] string authorization)
        {
            try {
                Book book = GetBook(userId, bookId, authorization);
                Account account = Repo.FindAccount(accountId);
                if (account.Book.Id != book.Id) return HttpNotFound(new { error = "Account [" + accountId + "] not found in Book [" + bookId + "]" });
                return new ObjectResult(account);
            }
            catch (PiggyBankUserException e) { return HttpUnauthorized(); }
            catch (PiggyBankBookException e) { return HttpNotFound(new { error = e.Message }); }
            catch (PiggyBankDataException e) { return HttpBadRequest(new { error = e.Message }); }
        }

        [HttpPut("{accountId}")]
        public IActionResult Put(int userId, int bookId, int accountId, [FromBody] Account account, [FromHeader] string authorization)
        {
            try
            {
                if (account == null) return HttpBadRequest(new { error = "Account object not provided" });
                if (account.Id != accountId) return HttpBadRequest(new { error = "Invalid Account.Id [" + account.Id + "]" });
                Book book = GetBook(userId, bookId, authorization);
                Account accountToUpdate = Repo.FindAccount(account.Id);
                if (accountToUpdate == null || accountToUpdate.Book.Id != book.Id) return HttpNotFound(new { error = "Account [" + account.Id + "] not found in Book [" + bookId + "]" });
                Repo.UpdateAccount(account);
                return new NoContentResult();
            }
            catch (PiggyBankUserException e) { return HttpUnauthorized(); }
            catch (PiggyBankBookException e) { return HttpNotFound(new { error = e.Message }); }
            catch (PiggyBankDataException e) { return HttpBadRequest(new { error = e.Message }); }
        }

        private Book GetBook(int userId, int bookId, string authorization)
        {
            User user = TokenRequirement.Fulfill(Repo, userId, authorization);
            if (user == null) throw new PiggyBankUserException("Unknown error");
            Book book = Repo.FindBook(bookId);
            if (book == null || book.User.Id != userId) throw new PiggyBankBookException("Book [" + bookId + "] not found in User [" + userId + "]");
            return book;
        }
    }
    
}
