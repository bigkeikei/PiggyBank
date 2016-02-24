using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;

using SimpleIdentity.Models;
using PiggyBank.Models;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace PiggyBank.Controllers
{
    [Route("api/users/{userId}")]
    public class AccountsController : Controller
    {
        [FromServices]
        public IPiggyBankRepository Repo { get; set; }

        [FromServices]
        public ISimpleIdentityRepository IdentityRepo { get; set; }

        [HttpGet("books/{bookId}/[controller]")]
        public async Task<IActionResult> List(int userId, int bookId, [FromHeader] string authorization)
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
                Book book = await GetBook(userId, bookId);
                IEnumerable<Account> accounts = await Repo.AccountManager.ListAccounts(book);
                return new ObjectResult(accounts);
            }
            catch (TokenExtractionException) { return HttpUnauthorized(); }
            catch (PiggyBankBookException) { return HttpUnauthorized(); }
        }

        [HttpGet("[controller]")]
        public async Task<ActionResult> List(int userId, [FromHeader] string authorization)
        {
            AuthorizationRequirement req = new AuthorizationRequirement
            {
                AuthResourceType = Authorization.AuthResourceType.User,
                ResourceId = userId,
                Scopes = Authorization.AuthScopes.Readable
            };
            try
            {
                WebAuthorizationHandler authHandler = new WebAuthorizationHandler(authorization);
                if (!await authHandler.IsValid(IdentityRepo, Request.Method, Request.Path)) { return HttpUnauthorized(); }
                if (!await authHandler.FulFill(IdentityRepo, req)) { return HttpUnauthorized(); }
                IEnumerable<Account> accounts = await Repo.AccountManager.ListAccounts(userId);
                List<Account> readableAccounts = new List<Account>();
                foreach ( Account account in accounts)
                {
                    List<AuthorizationRequirement> accReqs = new List<AuthorizationRequirement>();
                    accReqs.Add(new AuthorizationRequirement
                    {
                        AuthResourceType = Authorization.AuthResourceType.User,
                        ResourceId = userId,
                        Scopes = Authorization.AuthScopes.Full
                    });
                    accReqs.Add(new AuthorizationRequirement
                    {
                        AuthResourceType = Authorization.AuthResourceType.Book,
                        ResourceId = account.Book.Id,
                        Scopes = Authorization.AuthScopes.Readable
                    });
                    accReqs.Add(new AuthorizationRequirement
                    {
                        AuthResourceType = Authorization.AuthResourceType.Account,
                        ResourceId = account.Id,
                        Scopes = Authorization.AuthScopes.Readable
                    });
                    if (await authHandler.FulFillAny(IdentityRepo, accReqs)) { readableAccounts.Add(account); }
                }
                return new ObjectResult(readableAccounts);
            }
            catch (TokenExtractionException) { return HttpUnauthorized(); }
            catch (PiggyBankBookException) { return HttpUnauthorized(); }
        }

        [HttpPost("books/{bookId}/[controller]")]
        public async Task<IActionResult> Post(int userId, int bookId, [FromBody] Account account, [FromHeader] string authorization)
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
                Book book = await GetBook(userId, bookId);
                Account accountCreated = await Repo.AccountManager.CreateAccount(book, account);
                return CreatedAtRoute("GetAccount", new { controller = "accounts", userId = userId, bookId = bookId, accountId = accountCreated.Id }, accountCreated);
            }
            catch (TokenExtractionException) { return HttpUnauthorized(); }
            catch (PiggyBankDataNotFoundException) { return HttpUnauthorized(); }
            catch (PiggyBankBookException) { return HttpUnauthorized(); }
            catch (PiggyBankDataException e) { return HttpBadRequest(new { error = e.Message }); }
        }

        [HttpGet("[controller]/{accountId}", Name = "GetAccount")]
        public async Task<IActionResult> Get(int userId, int accountId, [FromHeader] string authorization)
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
                AuthResourceType = Authorization.AuthResourceType.Account,
                ResourceId = accountId,
                Scopes = Authorization.AuthScopes.Readable
            });
            try {
                WebAuthorizationHandler authHandler = new WebAuthorizationHandler(authorization);
                if (!await authHandler.IsValid(IdentityRepo, Request.Method, Request.Path)) { return HttpUnauthorized(); }
                Account account = await Repo.AccountManager.FindAccount(accountId, userId);
                reqs.Add(new AuthorizationRequirement
                {
                    AuthResourceType = Authorization.AuthResourceType.Book,
                    ResourceId = account.Book.Id,
                    Scopes = Authorization.AuthScopes.Readable
                });
                if (!await authHandler.FulFillAny(IdentityRepo, reqs)) { return HttpUnauthorized(); }
                return new ObjectResult(account);
            }
            catch (TokenExtractionException) { return HttpUnauthorized(); }
            catch (PiggyBankDataNotFoundException) { return HttpUnauthorized(); }
            catch (PiggyBankDataException e) { return HttpBadRequest(new { error = e.Message }); }
        }

        [HttpPut("[controller]/{accountId}")]
        public async Task<IActionResult> Put(int userId, int accountId, [FromBody] Account account, [FromHeader] string authorization)
        {
            List<AuthorizationRequirement> reqs = new List<AuthorizationRequirement>();
            reqs.Add(new AuthorizationRequirement
            {
                AuthResourceType = Authorization.AuthResourceType.User,
                ResourceId = userId,
                Scopes = Authorization.AuthScopes.Full
            });
            try
            {
                WebAuthorizationHandler authHandler = new WebAuthorizationHandler(authorization);
                if (!await authHandler.IsValid(IdentityRepo, Request.Method, Request.Path)) { return HttpUnauthorized(); }
                if (account == null) return HttpBadRequest(new { error = "Account object is missing" });
                if (account.Id != accountId) return HttpBadRequest(new { error = "Invalid Account.Id [" + account.Id + "]" });
                Account accountToUpdate = await Repo.AccountManager.FindAccount(accountId, userId);
                //if (accountToUpdate.Book.Id != bookId) return HttpNotFound(new { error = "Account [" + account.Id + "] not found in Book [" + bookId + "]" });
                reqs.Add(new AuthorizationRequirement
                {
                    AuthResourceType = Authorization.AuthResourceType.Book,
                    ResourceId = accountToUpdate.Book.Id,
                    Scopes = Authorization.AuthScopes.Editable
                });
                if (!await authHandler.FulFillAny(IdentityRepo, reqs)) { return HttpUnauthorized(); }
                await Repo.AccountManager.UpdateAccount(account);
                return new NoContentResult();
            }
            catch (TokenExtractionException) { return HttpUnauthorized(); }
            catch (PiggyBankDataNotFoundException) { return HttpUnauthorized(); }
            catch (PiggyBankDataException e) { return HttpBadRequest(new { error = e.Message }); }
        }

        [HttpGet("[controller]/{accountId}/detail")]
        public async Task<IActionResult> GetDetail(int userId, int accountId, [FromHeader] string authorization)
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
                AuthResourceType = Authorization.AuthResourceType.Account,
                ResourceId = accountId,
                Scopes = Authorization.AuthScopes.Readable
            });
            try
            {
                WebAuthorizationHandler authHandler = new WebAuthorizationHandler(authorization);
                if (!await authHandler.IsValid(IdentityRepo, Request.Method, Request.Path)) { return HttpUnauthorized(); }
                Account account = await Repo.AccountManager.FindAccount(accountId, userId);
                reqs.Add(new AuthorizationRequirement
                {
                    AuthResourceType = Authorization.AuthResourceType.Book,
                    ResourceId = account.Book.Id,
                    Scopes = Authorization.AuthScopes.Readable
                });
                if (!await authHandler.FulFillAny(IdentityRepo, reqs)) { return HttpUnauthorized(); }
                return new ObjectResult(await Repo.AccountManager.GetAccountDetail(accountId));
            }
            catch (TokenExtractionException) { return HttpUnauthorized(); }
            catch (PiggyBankDataNotFoundException) { return HttpUnauthorized(); }
            catch (PiggyBankDataException e) { return HttpBadRequest(new { error = e.Message }); }
        }

        [HttpGet("[controller]/{accountId}/transactions")]
        public async Task<IActionResult> GetTransactions(int userId, int accountId, [FromQuery] DateTime? periodStart, [FromQuery] DateTime? periodEnd, [FromQuery] int? noOfRecords, [FromHeader] string authorization)
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
                AuthResourceType = Authorization.AuthResourceType.Account,
                ResourceId = accountId,
                Scopes = Authorization.AuthScopes.Readable
            });
            try
            {
                WebAuthorizationHandler authHandler = new WebAuthorizationHandler(authorization);
                if (!await authHandler.IsValid(IdentityRepo, Request.Method, Request.Path)) { return HttpUnauthorized(); }
                Account account = await Repo.AccountManager.FindAccount(accountId, userId);
                reqs.Add(new AuthorizationRequirement
                {
                    AuthResourceType = Authorization.AuthResourceType.Book,
                    ResourceId = account.Book.Id,
                    Scopes = Authorization.AuthScopes.Readable
                });
                if (!await authHandler.FulFillAny(IdentityRepo, reqs)) { return HttpUnauthorized(); }
                return new ObjectResult(await Repo.AccountManager.GetTransactions(accountId, periodStart, periodEnd, noOfRecords));
            }
            catch (TokenExtractionException) { return HttpUnauthorized(); }
            catch (PiggyBankDataNotFoundException) { return HttpUnauthorized(); }
            catch (PiggyBankDataException e) { return HttpBadRequest(new { error = e.Message }); }
        }

        [HttpGet("[controller]/{accountId}/transactions/count")]
        public async Task<IActionResult> CountTransactions(int userId, int accountId, [FromQuery] DateTime? periodStart, [FromQuery] DateTime? periodEnd, [FromHeader] string authorization)
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
                AuthResourceType = Authorization.AuthResourceType.Account,
                ResourceId = accountId,
                Scopes = Authorization.AuthScopes.Readable
            });
            try
            {
                WebAuthorizationHandler authHandler = new WebAuthorizationHandler(authorization);
                if (!await authHandler.IsValid(IdentityRepo, Request.Method, Request.Path)) { return HttpUnauthorized(); }
                Account account = await Repo.AccountManager.FindAccount(accountId, userId);
                reqs.Add(new AuthorizationRequirement
                {
                    AuthResourceType = Authorization.AuthResourceType.Book,
                    ResourceId = account.Book.Id,
                    Scopes = Authorization.AuthScopes.Readable
                });
                if (!await authHandler.FulFillAny(IdentityRepo, reqs)) { return HttpUnauthorized(); }
                return new ObjectResult(await Repo.AccountManager.GetTransactionCount(accountId, periodStart, periodEnd));
            }
            catch (TokenExtractionException) { return HttpUnauthorized(); }
            catch (PiggyBankDataNotFoundException) { return HttpUnauthorized(); }
            catch (PiggyBankDataException e) { return HttpBadRequest(new { error = e.Message }); }
        }

        private async Task<Book> GetBook(int userId, int bookId)
        {
            Book book = await Repo.BookManager.FindBook(bookId);
            if (book.UserId != userId) throw new PiggyBankBookException("Book [" + bookId + "] not found in User [" + userId + "]");
            return book;
        }
    }
    
}
