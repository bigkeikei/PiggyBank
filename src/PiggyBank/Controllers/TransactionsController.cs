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
    [Route("api/users/{userId}/books/{bookId}/[controller]")]
    public class TransactionsController : Controller
    {
        [FromServices]
        public IPiggyBankRepository Repo { get; set; }

        [HttpGet("{transactionId}", Name = "GetTransaction")]
        public IActionResult Get(int userId, int bookId, int transactionId, [FromHeader] string authorization)
        {
            try
            {
                Book book = GetBook(userId, bookId, authorization);
                Transaction transaction = Repo.TransactionManager.FindTransaction(transactionId);
                if (transaction.Book.Id != bookId) return HttpUnauthorized();
                return new ObjectResult(transaction);
            }
            catch (PiggyBankUserException e) { return HttpUnauthorized(); }
            catch (PiggyBankBookException e) { return HttpUnauthorized(); }
            catch (PiggyBankDataNotFoundException e) { return HttpUnauthorized(); }
            catch (PiggyBankDataException e) { return HttpBadRequest(new { error = e.Message }); }
        }

        [HttpPost]
        public IActionResult Post(int userId, int bookId, [FromBody]Transaction transaction, [FromHeader] string authorization)
        {
            try
            {
                Book book = GetBook(userId, bookId, authorization);
                Transaction transactionCreated = Repo.TransactionManager.CreateTransaction(book, transaction);
                return CreatedAtRoute("GetTransaction", new { controller = "transactions", userId = userId, bookId = bookId, transactionId = transactionCreated.Id }, transactionCreated);

            }
            catch (PiggyBankUserException e) { return HttpUnauthorized(); }
            catch (PiggyBankBookException e) { return HttpUnauthorized(); }
            catch (PiggyBankDataNotFoundException e) { return HttpUnauthorized(); }
            catch (PiggyBankDataException e) { return HttpBadRequest(new { error = e.Message }); }
        }

        [HttpPut("{transactionId}")]
        public IActionResult Put(int userId, int bookId, int transactionId, [FromBody]Transaction transaction, [FromHeader] string authorization)
        {
            try
            {
                if (transaction == null) return HttpBadRequest(new { error = "Transaction object is missing" });
                if (transaction.Id != transactionId) return HttpBadRequest(new { error = "Invalid Transaction.Id [" + transaction.Id + "]" });
                Book book = GetBook(userId, bookId, authorization);
                Transaction transactionToUpdate = Repo.TransactionManager.FindTransaction(transactionId);
                if (transactionToUpdate.Book.Id != bookId) return HttpBadRequest(new { error = "Transaction [" + transactionId + "] cannot be found in Book [" + bookId + "]" });
                Repo.TransactionManager.UpdateTransaction(transaction);
                return new NoContentResult();
            }
            catch (PiggyBankUserException e) { return HttpUnauthorized(); }
            catch (PiggyBankBookException e) { return HttpUnauthorized(); }
            catch (PiggyBankDataNotFoundException e) { return HttpUnauthorized(); }
            catch (PiggyBankDataException e) { return HttpBadRequest(new { error = e.Message }); }
        }

        private Book GetBook(int userId, int bookId, string authorization)
        {
            User user = TokenRequirement.Fulfill(Repo, userId, authorization);
            if (user == null) throw new PiggyBankUserException("Unknown error");
            Book book = Repo.BookManager.FindBook(bookId);
            if (book == null || book.User.Id != userId) throw new PiggyBankBookException("Book [" + bookId + "] not found in User [" + userId + "]");
            return book;
        }
    }
}
