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
    [Route("api/[controller]")]
    public class BooksController : Controller
    {
        [FromServices]
        public IPiggyBankRepository Repo { get; set; }

        [FromServices]
        public ISimpleIdentityRepository IdentityRepo { get; set; }

        [HttpGet("{bookId}", Name ="GetBook")]
        public async Task<IActionResult> Get(int bookId, [FromHeader] string authorization)
        {
            try
            {
                Book book = await Repo.BookManager.FindBook(bookId);
                List<AuthorizationRequirement> reqs = new List<AuthorizationRequirement>();
                reqs.Add(new AuthorizationRequirement
                {
                    AuthResourceType = Authorization.AuthResourceType.User,
                    ResourceId = book.UserId,
                    Scopes = Authorization.AuthScopes.Full
                });
                reqs.Add(new AuthorizationRequirement
                {
                    AuthResourceType = Authorization.AuthResourceType.Book,
                    ResourceId = bookId,
                    Scopes = Authorization.AuthScopes.Readable
                });
                WebAuthorizationHandler authHandler = new WebAuthorizationHandler(authorization);
                if (!await authHandler.IsValid(IdentityRepo, Request.Method, Request.Path)) { return HttpUnauthorized(); }
                if (!await authHandler.FulFillAny(IdentityRepo, reqs)) { return HttpUnauthorized(); }
                return new ObjectResult(book);
            }
            catch (TokenExtractionException) { return HttpUnauthorized(); }
            catch (PiggyBankDataNotFoundException) { return HttpUnauthorized(); }
            catch (PiggyBankDataException e) { return HttpBadRequest(new { error = e.Message }); }
        }

        [HttpPut("{bookId}")]
        public async Task<IActionResult> Put(int bookId, [FromBody] Book book, [FromHeader] string authorization)
        {
            if (book == null) return HttpBadRequest(new { error = "Book object not provided" });
            if (book.Id != bookId) return HttpBadRequest(new { error = "Invalid Book.Id [" + book.Id + "]" });

            try
            {
                Book bookToUpdate = await Repo.BookManager.FindBook(bookId);
                List<AuthorizationRequirement> reqs = new List<AuthorizationRequirement>();
                reqs.Add(new AuthorizationRequirement
                {
                    AuthResourceType = Authorization.AuthResourceType.User,
                    ResourceId = bookToUpdate.UserId,
                    Scopes = Authorization.AuthScopes.Full
                });
                reqs.Add(new AuthorizationRequirement
                {
                    AuthResourceType = Authorization.AuthResourceType.Book,
                    ResourceId = bookId,
                    Scopes = Authorization.AuthScopes.Editable
                }); WebAuthorizationHandler authHandler = new WebAuthorizationHandler(authorization);
                if (!await authHandler.IsValid(IdentityRepo, Request.Method, Request.Path, Request.Body)) { return HttpUnauthorized(); }
                if (!await authHandler.FulFillAny(IdentityRepo, reqs)) { return HttpUnauthorized(); }
                await Repo.BookManager.UpdateBook(book);
                return new NoContentResult();
            }
            catch (TokenExtractionException) { return HttpUnauthorized(); }
            catch (PiggyBankDataNotFoundException) { return HttpUnauthorized(); }
            catch (PiggyBankDataException e) { return HttpBadRequest(new { error = e.Message }); }
        }

        [HttpGet("{bookId}/accounts")]
        public async Task<IActionResult> GetAccounts(int bookId, [FromHeader] string authorization)
        {
            try
            {
                Book book = await Repo.BookManager.FindBook(bookId);
                List<AuthorizationRequirement> reqs = new List<AuthorizationRequirement>();
                reqs.Add(new AuthorizationRequirement
                {
                    AuthResourceType = Authorization.AuthResourceType.User,
                    ResourceId = book.UserId,
                    Scopes = Authorization.AuthScopes.Full
                });
                reqs.Add(new AuthorizationRequirement
                {
                    AuthResourceType = Authorization.AuthResourceType.Book,
                    ResourceId = book.Id,
                    Scopes = Authorization.AuthScopes.Readable
                });
                WebAuthorizationHandler authHandler = new WebAuthorizationHandler(authorization);
                if (!await authHandler.IsValid(IdentityRepo, Request.Method, Request.Path)) { return HttpUnauthorized(); }
                if (!await authHandler.FulFillAny(IdentityRepo, reqs)) { return HttpUnauthorized(); }
                IEnumerable<Account> accounts = await Repo.AccountManager.ListAccounts(book);
                return new ObjectResult(accounts);
            }
            catch (TokenExtractionException) { return HttpUnauthorized(); }
            catch (PiggyBankBookException) { return HttpUnauthorized(); }
        }

        [HttpPost("{bookId}/accounts")]
        public async Task<IActionResult> PostAccount(int bookId, [FromBody] Account account, [FromHeader] string authorization)
        {
            try
            {
                Book book = await Repo.BookManager.FindBook(bookId);
                List<AuthorizationRequirement> reqs = new List<AuthorizationRequirement>();
                reqs.Add(new AuthorizationRequirement
                {
                    AuthResourceType = Authorization.AuthResourceType.User,
                    ResourceId = book.UserId,
                    Scopes = Authorization.AuthScopes.Full
                });
                reqs.Add(new AuthorizationRequirement
                {
                    AuthResourceType = Authorization.AuthResourceType.Book,
                    ResourceId = bookId,
                    Scopes = Authorization.AuthScopes.Editable
                });
                WebAuthorizationHandler authHandler = new WebAuthorizationHandler(authorization);
                if (!await authHandler.IsValid(IdentityRepo, Request.Method, Request.Path, Request.Body)) { return HttpUnauthorized(); }
                if (!await authHandler.FulFillAny(IdentityRepo, reqs)) { return HttpUnauthorized(); }
                Account accountCreated = await Repo.AccountManager.CreateAccount(book, account);
                return CreatedAtRoute("GetAccount", new { controller = "accounts", accountId = accountCreated.Id }, accountCreated);
            }
            catch (TokenExtractionException) { return HttpUnauthorized(); }
            catch (PiggyBankDataNotFoundException) { return HttpUnauthorized(); }
            catch (PiggyBankBookException) { return HttpUnauthorized(); }
            catch (PiggyBankDataException e) { return HttpBadRequest(new { error = e.Message }); }
        }

        [HttpGet("{bookId}/transactions")]
        public async Task<ActionResult> GetTransactions(int bookId, [FromQuery] DateTime? periodStart, [FromQuery] DateTime? periodEnd, [FromQuery] int? noOfRecords, [FromHeader] string authorization)
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            if (periodStart != null) { args.Add("periodStart", Request.Query["periodStart"]); }
            if (periodEnd != null) { args.Add("periodEnd", Request.Query["periodEnd"]); }
            if (noOfRecords != null) { args.Add("noOfRecords", Request.Query["noOfRecords"]); }

            try
            {
                Book book = await Repo.BookManager.FindBook(bookId);
                List<AuthorizationRequirement> reqs = new List<AuthorizationRequirement>();
                reqs.Add(new AuthorizationRequirement
                {
                    AuthResourceType = Authorization.AuthResourceType.User,
                    ResourceId = book.UserId,
                    Scopes = Authorization.AuthScopes.Full
                });
                reqs.Add(new AuthorizationRequirement
                {
                    AuthResourceType = Authorization.AuthResourceType.Book,
                    ResourceId = bookId,
                    Scopes = Authorization.AuthScopes.Readable
                });
                WebAuthorizationHandler authHandler = new WebAuthorizationHandler(authorization);
                if (!await authHandler.IsValid(IdentityRepo, Request.Method, Request.Path, parameters: args)) { return HttpUnauthorized(); }
                if (!await authHandler.FulFillAny(IdentityRepo, reqs)) { return HttpUnauthorized(); }
                return new ObjectResult(await Repo.TransactionManager.ListTransactions(bookId, periodStart, periodEnd, noOfRecords));

            }
            catch (TokenExtractionException) { return HttpUnauthorized(); }
            catch (PiggyBankDataNotFoundException) { return HttpUnauthorized(); }
            catch (PiggyBankDataException e) { return HttpBadRequest(new { error = e.Message }); }
        }

        [HttpGet("{bookId}/transactions/count")]
        public async Task<ActionResult> GetTransationCount(int bookId, [FromQuery] DateTime? periodStart, [FromQuery] DateTime? periodEnd, [FromHeader] string authorization)
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            if (periodStart != null) { args.Add("periodStart", Request.Query["periodStart"]); }
            if (periodEnd != null) { args.Add("periodEnd", Request.Query["periodEnd"]); }

            try
            {
                Book book = await Repo.BookManager.FindBook(bookId);
                List<AuthorizationRequirement> reqs = new List<AuthorizationRequirement>();
                reqs.Add(new AuthorizationRequirement
                {
                    AuthResourceType = Authorization.AuthResourceType.User,
                    ResourceId = book.UserId,
                    Scopes = Authorization.AuthScopes.Full
                });
                reqs.Add(new AuthorizationRequirement
                {
                    AuthResourceType = Authorization.AuthResourceType.Book,
                    ResourceId = bookId,
                    Scopes = Authorization.AuthScopes.Readable
                });
                WebAuthorizationHandler authHandler = new WebAuthorizationHandler(authorization);
                if (!await authHandler.IsValid(IdentityRepo, Request.Method, Request.Path, parameters: args)) { return HttpUnauthorized(); }
                if (!await authHandler.FulFillAny(IdentityRepo, reqs)) { return HttpUnauthorized(); }
                return new ObjectResult(await Repo.TransactionManager.CountTransactions(bookId, periodStart, periodEnd));

            }
            catch (TokenExtractionException) { return HttpUnauthorized(); }
            catch (PiggyBankDataNotFoundException) { return HttpUnauthorized(); }
            catch (PiggyBankDataException e) { return HttpBadRequest(new { error = e.Message }); }
        }

        [HttpPost("{bookId}/transactions")]
        public async Task<IActionResult> PostTransactions(int bookId, [FromBody]Transaction transaction, [FromHeader] string authorization)
        {
            try
            {
                Book book = await Repo.BookManager.FindBook(bookId);
                List<AuthorizationRequirement> reqs = new List<AuthorizationRequirement>();
                reqs.Add(new AuthorizationRequirement
                {
                    AuthResourceType = Authorization.AuthResourceType.User,
                    ResourceId = book.UserId,
                    Scopes = Authorization.AuthScopes.Full
                });
                reqs.Add(new AuthorizationRequirement
                {
                    AuthResourceType = Authorization.AuthResourceType.Book,
                    ResourceId = bookId,
                    Scopes = Authorization.AuthScopes.Editable
                });
                WebAuthorizationHandler authHandler = new WebAuthorizationHandler(authorization);
                if (!await authHandler.IsValid(IdentityRepo, Request.Method, Request.Path, Request.Body)) { return HttpUnauthorized(); }
                if (!await authHandler.FulFillAny(IdentityRepo, reqs)) { return HttpUnauthorized(); }
                Transaction transactionCreated = await Repo.TransactionManager.CreateTransaction(book, transaction);
                return CreatedAtRoute("GetTransaction", new { controller = "transactions", transactionId = transactionCreated.Id }, transactionCreated);

            }
            catch (TokenExtractionException) { return HttpUnauthorized(); }
            catch (PiggyBankDataNotFoundException) { return HttpUnauthorized(); }
            catch (PiggyBankDataException e) { return HttpBadRequest(new { error = e.Message }); }
        }

        [HttpGet("{bookId}/tags")]
        public async Task<IActionResult> GetTags(int bookId, [FromHeader] string authorization)
        {
            try
            {
                Book book = await Repo.BookManager.FindBook(bookId);
                List<AuthorizationRequirement> reqs = new List<AuthorizationRequirement>();
                reqs.Add(new AuthorizationRequirement
                {
                    AuthResourceType = Authorization.AuthResourceType.User,
                    ResourceId = book.UserId,
                    Scopes = Authorization.AuthScopes.Full
                });
                reqs.Add(new AuthorizationRequirement
                {
                    AuthResourceType = Authorization.AuthResourceType.Book,
                    ResourceId = bookId,
                    Scopes = Authorization.AuthScopes.Readable
                });
                WebAuthorizationHandler authHandler = new WebAuthorizationHandler(authorization);
                if (!await authHandler.IsValid(IdentityRepo, Request.Method, Request.Path)) { return HttpUnauthorized(); }
                if (!await authHandler.FulFillAny(IdentityRepo, reqs)) { return HttpUnauthorized(); }
                return new ObjectResult(await Repo.TagManager.ListTags(bookId));
            }
            catch (TokenExtractionException) { return HttpUnauthorized(); }
            catch (PiggyBankDataException e) { return HttpBadRequest(new { error = e.Message }); }
        }

        [HttpPost("{bookId}/tags")]
        public async Task<IActionResult> PostTag(int bookId, [FromBody] Tag tag, [FromHeader] string authorization)
        {
            try
            {
                Book book = await Repo.BookManager.FindBook(bookId);
                List<AuthorizationRequirement> reqs = new List<AuthorizationRequirement>();
                reqs.Add(new AuthorizationRequirement
                {
                    AuthResourceType = Authorization.AuthResourceType.User,
                    ResourceId = book.UserId,
                    Scopes = Authorization.AuthScopes.Full
                });
                reqs.Add(new AuthorizationRequirement
                {
                    AuthResourceType = Authorization.AuthResourceType.Book,
                    ResourceId = bookId,
                    Scopes = Authorization.AuthScopes.Editable
                });
                WebAuthorizationHandler authHandler = new WebAuthorizationHandler(authorization);
                if (!await authHandler.IsValid(IdentityRepo, Request.Method, Request.Path, Request.Body)) { return HttpUnauthorized(); }
                if (!await authHandler.FulFillAny(IdentityRepo, reqs)) { return HttpUnauthorized(); }
                Tag tagCreated = await Repo.TagManager.CreateTag(book, tag);
                return CreatedAtRoute("GetTag", new { controller = "tags", tagId = tagCreated.Id }, tagCreated);
            }
            catch (TokenExtractionException) { return HttpUnauthorized(); }
            catch (PiggyBankDataNotFoundException) { return HttpUnauthorized(); }
            catch (PiggyBankDataException e) { return HttpBadRequest(new { error = e.Message }); }
        }
    }
}
