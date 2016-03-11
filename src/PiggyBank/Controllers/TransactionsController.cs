using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;

using PiggyBank.Models;
using SimpleIdentity.Models;
// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace PiggyBank.Controllers
{
    [Route("api/users/{userId}/books/{bookId}/[controller]")]
    public class TransactionsController : Controller
    {
        [FromServices]
        public IPiggyBankRepository Repo { get; set; }

        [FromServices]
        public ISimpleIdentityRepository IdentityRepo { get; set; }

        [HttpGet]
        public async Task<ActionResult> Get(int userId, int bookId, [FromQuery] DateTime? periodStart, [FromQuery] DateTime? periodEnd, [FromQuery] int? noOfRecords, [FromHeader] string authorization)
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            if (periodStart != null) { args.Add("periodStart", Request.Query["periodStart"]); }
            if (periodEnd != null) { args.Add("periodEnd", Request.Query["periodEnd"]); }
            if (noOfRecords != null) { args.Add("noOfRecords", Request.Query["noOfRecords"]); }

            List<AuthorizationRequirement> reqs = new List<AuthorizationRequirement>();
            reqs.Add(new AuthorizationRequirement
            {
                AuthResourceType = Authorization.AuthResourceType.User,
                ResourceId = userId,
                Scopes = Authorization.AuthScopes.Full
            });
            reqs.Add(new AuthorizationRequirement
            {
                AuthResourceType = Authorization.AuthResourceType.Book,
                ResourceId = bookId,
                Scopes = Authorization.AuthScopes.Readable
            });
            try
            {
                WebAuthorizationHandler authHandler = new WebAuthorizationHandler(authorization);

                if (!await authHandler.IsValid(IdentityRepo, Request.Method, Request.Path, parameters: args)) { return HttpUnauthorized(); }
                if (!await authHandler.FulFillAny(IdentityRepo, reqs)) { return HttpUnauthorized(); }
                await GetBook(userId, bookId);
                return new ObjectResult(await Repo.TransactionManager.ListTransactions(bookId, periodStart, periodEnd, noOfRecords));

            }
            catch (TokenExtractionException) { return HttpUnauthorized(); }
            catch (PiggyBankBookException) { return HttpUnauthorized(); }
            catch (PiggyBankDataNotFoundException) { return HttpUnauthorized(); }
            catch (PiggyBankDataException e) { return HttpBadRequest(new { error = e.Message }); }
        }

        [HttpGet("count")]
        public async Task<ActionResult> GetCount(int userId, int bookId, [FromQuery] DateTime? periodStart, [FromQuery] DateTime? periodEnd, [FromHeader] string authorization)
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            if (periodStart != null) { args.Add("periodStart", Request.Query["periodStart"]); }
            if (periodEnd != null) { args.Add("periodEnd", Request.Query["periodEnd"]); }

            List<AuthorizationRequirement> reqs = new List<AuthorizationRequirement>();
            reqs.Add(new AuthorizationRequirement
            {
                AuthResourceType = Authorization.AuthResourceType.User,
                ResourceId = userId,
                Scopes = Authorization.AuthScopes.Full
            });
            reqs.Add(new AuthorizationRequirement
            {
                AuthResourceType = Authorization.AuthResourceType.Book,
                ResourceId = bookId,
                Scopes = Authorization.AuthScopes.Readable
            });
            try
            {
                WebAuthorizationHandler authHandler = new WebAuthorizationHandler(authorization);
                if (!await authHandler.IsValid(IdentityRepo, Request.Method, Request.Path, parameters: args)) { return HttpUnauthorized(); }
                if (!await authHandler.FulFillAny(IdentityRepo, reqs)) { return HttpUnauthorized(); }
                await GetBook(userId, bookId);
                return new ObjectResult(await Repo.TransactionManager.CountTransactions(bookId, periodStart, periodEnd));

            }
            catch (TokenExtractionException) { return HttpUnauthorized(); }
            catch (PiggyBankBookException) { return HttpUnauthorized(); }
            catch (PiggyBankDataNotFoundException) { return HttpUnauthorized(); }
            catch (PiggyBankDataException e) { return HttpBadRequest(new { error = e.Message }); }
        }

        [HttpGet("{transactionId}", Name = "GetTransaction")]
        public async Task<IActionResult> Get(int userId, int bookId, int transactionId, [FromHeader] string authorization)
        {
            List<AuthorizationRequirement> reqs = new List<AuthorizationRequirement>();
            reqs.Add(new AuthorizationRequirement
            {
                AuthResourceType = Authorization.AuthResourceType.User,
                ResourceId = userId,
                Scopes = Authorization.AuthScopes.Full
            });
            reqs.Add(new AuthorizationRequirement
            {
                AuthResourceType = Authorization.AuthResourceType.Book,
                ResourceId = bookId,
                Scopes = Authorization.AuthScopes.Readable
            });
            try
            {
                WebAuthorizationHandler authHandler = new WebAuthorizationHandler(authorization);
                if (!await authHandler.IsValid(IdentityRepo, Request.Method, Request.Path)) { return HttpUnauthorized(); }
                if (!await authHandler.FulFillAny(IdentityRepo, reqs)) { return HttpUnauthorized(); }
                Transaction transaction = await Repo.TransactionManager.FindTransaction(transactionId);
                Book book = await GetBook(userId, bookId);
                if (transaction.Book.Id != bookId) return HttpUnauthorized();
                return new ObjectResult(transaction);
            }
            catch (TokenExtractionException) { return HttpUnauthorized(); }
            catch (PiggyBankBookException) { return HttpUnauthorized(); }
            catch (PiggyBankDataNotFoundException) { return HttpUnauthorized(); }
            catch (PiggyBankDataException e) { return HttpBadRequest(new { error = e.Message }); }
        }

        [HttpPost]
        public async Task<IActionResult> Post(int userId, int bookId, [FromBody]Transaction transaction, [FromHeader] string authorization)
        {
            List<AuthorizationRequirement> reqs = new List<AuthorizationRequirement>();
            reqs.Add(new AuthorizationRequirement
            {
                AuthResourceType = Authorization.AuthResourceType.User,
                ResourceId = userId,
                Scopes = Authorization.AuthScopes.Full
            });
            reqs.Add(new AuthorizationRequirement
            {
                AuthResourceType = Authorization.AuthResourceType.Book,
                ResourceId = bookId,
                Scopes = Authorization.AuthScopes.Editable
            });
            try
            {
                WebAuthorizationHandler authHandler = new WebAuthorizationHandler(authorization);
                if (!await authHandler.IsValid(IdentityRepo, Request.Method, Request.Path, Request.Body)) { return HttpUnauthorized(); }
                if (!await authHandler.FulFillAny(IdentityRepo, reqs)) { return HttpUnauthorized(); }
                Book book = await GetBook(userId, bookId);
                Transaction transactionCreated = await Repo.TransactionManager.CreateTransaction(book, transaction);
                return CreatedAtRoute("GetTransaction", new { controller = "transactions", userId = userId, bookId = bookId, transactionId = transactionCreated.Id }, transactionCreated);

            }
            catch (TokenExtractionException) { return HttpUnauthorized(); }
            catch (PiggyBankBookException) { return HttpUnauthorized(); }
            catch (PiggyBankDataNotFoundException) { return HttpUnauthorized(); }
            catch (PiggyBankDataException e) { return HttpBadRequest(new { error = e.Message }); }
        }

        [HttpPut("{transactionId}")]
        public async Task<IActionResult> Put(int userId, int bookId, int transactionId, [FromBody]Transaction transaction, [FromHeader] string authorization)
        {
            List<AuthorizationRequirement> reqs = new List<AuthorizationRequirement>();
            reqs.Add(new AuthorizationRequirement
            {
                AuthResourceType = Authorization.AuthResourceType.User,
                ResourceId = userId,
                Scopes = Authorization.AuthScopes.Full
            });
            reqs.Add(new AuthorizationRequirement
            {
                AuthResourceType = Authorization.AuthResourceType.Book,
                ResourceId = bookId,
                Scopes = Authorization.AuthScopes.Editable
            });
            try
            {
                WebAuthorizationHandler authHandler = new WebAuthorizationHandler(authorization);
                if (!await authHandler.IsValid(IdentityRepo, Request.Method, Request.Path, Request.Body)) { return HttpUnauthorized(); }
                if (!await authHandler.FulFillAny(IdentityRepo, reqs)) { return HttpUnauthorized(); }
                if (transaction == null) return HttpBadRequest(new { error = "Transaction object is missing" });
                if (transaction.Id != transactionId) return HttpBadRequest(new { error = "Invalid Transaction.Id [" + transaction.Id + "]" });
                await GetBook(userId, bookId);
                Transaction transactionToUpdate = await Repo.TransactionManager.FindTransaction(transactionId);
                if (transactionToUpdate.Book.Id != bookId) return HttpBadRequest(new { error = "Transaction [" + transactionId + "] cannot be found in Book [" + bookId + "]" });
                await Repo.TransactionManager.UpdateTransaction(transaction);
                return new NoContentResult();
            }
            catch (TokenExtractionException) { return HttpUnauthorized(); }
            catch (PiggyBankBookException) { return HttpUnauthorized(); }
            catch (PiggyBankDataNotFoundException) { return HttpUnauthorized(); }
            catch (PiggyBankDataException e) { return HttpBadRequest(new { error = e.Message }); }
        }

        [HttpPost("{transactionId}/tags")]
        public async Task<IActionResult> PostTag(int userId, int bookId, int transactionId, [FromBody]Tag tag, [FromHeader] string authorization)
        {
            List<AuthorizationRequirement> reqs = new List<AuthorizationRequirement>();
            reqs.Add(new AuthorizationRequirement
            {
                AuthResourceType = Authorization.AuthResourceType.User,
                ResourceId = userId,
                Scopes = Authorization.AuthScopes.Full
            });
            reqs.Add(new AuthorizationRequirement
            {
                AuthResourceType = Authorization.AuthResourceType.Book,
                ResourceId = bookId,
                Scopes = Authorization.AuthScopes.Editable
            });
            try
            {
                WebAuthorizationHandler authHandler = new WebAuthorizationHandler(authorization);
                if (!await authHandler.IsValid(IdentityRepo, Request.Method, Request.Path, Request.Body)) { return HttpUnauthorized(); }
                if (!await authHandler.FulFillAny(IdentityRepo, reqs)) { return HttpUnauthorized(); }
                await GetBook(userId, bookId);
                await Repo.TransactionManager.AddTag(transactionId, tag);
                return new NoContentResult();
            }
            catch (TokenExtractionException) { return HttpUnauthorized(); }
            catch (PiggyBankBookException) { return HttpUnauthorized(); }
            catch (PiggyBankDataNotFoundException) { return HttpUnauthorized(); }
            catch (PiggyBankDataException e) { return HttpBadRequest(new { error = e.Message }); }
        }

        [HttpDelete("{transactionId}/tags/{tagId}")]
        public async Task<IActionResult> DeleteTag(int userId, int bookId, int transactionId, int tagId, [FromHeader] string authorization)
        {
            List<AuthorizationRequirement> reqs = new List<AuthorizationRequirement>();
            reqs.Add(new AuthorizationRequirement
            {
                AuthResourceType = Authorization.AuthResourceType.User,
                ResourceId = userId,
                Scopes = Authorization.AuthScopes.Full
            });
            reqs.Add(new AuthorizationRequirement
            {
                AuthResourceType = Authorization.AuthResourceType.Book,
                ResourceId = bookId,
                Scopes = Authorization.AuthScopes.Editable
            });
            try
            {
                WebAuthorizationHandler authHandler = new WebAuthorizationHandler(authorization);
                if (!await authHandler.IsValid(IdentityRepo, Request.Method, Request.Path)) { return HttpUnauthorized(); }
                if (!await authHandler.FulFillAny(IdentityRepo, reqs)) { return HttpUnauthorized(); }
                await GetBook(userId, bookId);
                await Repo.TransactionManager.RemoveTag(transactionId, tagId);
                return new NoContentResult();
            }
            catch (TokenExtractionException) { return HttpUnauthorized(); }
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
