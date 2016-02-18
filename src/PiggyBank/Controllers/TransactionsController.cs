using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;

using PiggyBank.Models;
// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace PiggyBank.Controllers
{
    [Route("api/users/{userId}/books/{bookId}/[controller]")]
    public class TransactionsController : Controller
    {
        [FromServices]
        public IPiggyBankRepository Repo { get; set; }

        [HttpGet("{transactionId}", Name = "GetTransaction")]
        public async Task<IActionResult> Get(int userId, int bookId, int transactionId, [FromHeader] string authorization)
        {
            try
            {
                Transaction transaction = await Repo.TransactionManager.FindTransaction(transactionId);
                if (transaction.Book.Id != bookId) return HttpUnauthorized();
                return new ObjectResult(transaction);
            }
            catch (PiggyBankBookException) { return HttpUnauthorized(); }
            catch (PiggyBankDataNotFoundException) { return HttpUnauthorized(); }
            catch (PiggyBankDataException e) { return HttpBadRequest(new { error = e.Message }); }
        }

        [HttpPost]
        public async Task<IActionResult> Post(int userId, int bookId, [FromBody]Transaction transaction, [FromHeader] string authorization)
        {
            try
            {
                Book book = await GetBook(userId, bookId);
                Transaction transactionCreated = await Repo.TransactionManager.CreateTransaction(book, transaction);
                return CreatedAtRoute("GetTransaction", new { controller = "transactions", userId = userId, bookId = bookId, transactionId = transactionCreated.Id }, transactionCreated);

            }
            catch (PiggyBankBookException) { return HttpUnauthorized(); }
            catch (PiggyBankDataNotFoundException) { return HttpUnauthorized(); }
            catch (PiggyBankDataException e) { return HttpBadRequest(new { error = e.Message }); }
        }

        [HttpPut("{transactionId}")]
        public async Task<IActionResult> Put(int userId, int bookId, int transactionId, [FromBody]Transaction transaction, [FromHeader] string authorization)
        {
            try
            {
                if (transaction == null) return HttpBadRequest(new { error = "Transaction object is missing" });
                if (transaction.Id != transactionId) return HttpBadRequest(new { error = "Invalid Transaction.Id [" + transaction.Id + "]" });
                Transaction transactionToUpdate = await Repo.TransactionManager.FindTransaction(transactionId);
                if (transactionToUpdate.Book.Id != bookId) return HttpBadRequest(new { error = "Transaction [" + transactionId + "] cannot be found in Book [" + bookId + "]" });
                await Repo.TransactionManager.UpdateTransaction(transaction);
                return new NoContentResult();
            }
            catch (PiggyBankBookException) { return HttpUnauthorized(); }
            catch (PiggyBankDataNotFoundException) { return HttpUnauthorized(); }
            catch (PiggyBankDataException e) { return HttpBadRequest(new { error = e.Message }); }
        }

        private async Task<Book> GetBook(int userId, int bookId)
        {
            Book book = await Repo.BookManager.FindBook(bookId);
            if (book == null || book.UserId != userId) throw new PiggyBankBookException("Book [" + bookId + "] not found in User [" + userId + "]");
            return book;
        }
    }
}
