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
    [Route("api/users/{userId}/books/{bookId}/[controller]")]
    public class AccountsController : Controller
    {
        [FromServices]
        public IPiggyBankRepository Repo { get; set; }

        [FromServices]
        public ISimpleIdentityRepository IdentityRepo { get; set; }

        [HttpGet]
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
                if (!await WebAuthorizationHandler.FulFillAny(IdentityRepo, authorization, reqs)) { return HttpUnauthorized(); }
                Book book = await GetBook(userId, bookId);
                IEnumerable<Account> accounts = await Repo.AccountManager.ListAccounts(book);
                return new ObjectResult(accounts);
            }
            catch (TokenExtractionException) { return HttpUnauthorized(); }
            catch (PiggyBankDataNotFoundException) { return HttpUnauthorized(); }
            catch (PiggyBankBookException) { return HttpUnauthorized(); }
        }

        [HttpPost]
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
                if (!await WebAuthorizationHandler.FulFillAny(IdentityRepo, authorization, reqs)) { return HttpUnauthorized(); }
                Book book = await GetBook(userId, bookId);
                Account accountCreated = await Repo.AccountManager.CreateAccount(book, account);
                return CreatedAtRoute("GetAccount", new { controller = "accounts", userId = userId, bookId = bookId, accountId = accountCreated.Id }, accountCreated);
            }
            catch (TokenExtractionException) { return HttpUnauthorized(); }
            catch (PiggyBankDataNotFoundException) { return HttpUnauthorized(); }
            catch (PiggyBankBookException) { return HttpUnauthorized(); }
            catch (PiggyBankDataException e) { return HttpBadRequest(new { error = e.Message }); }
        }

        [HttpGet("{accountId}", Name = "GetAccount")]
        public async Task<IActionResult> Get(int userId, int bookId, int accountId, [FromHeader] string authorization)
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
            reqs.Add(new AuthorizationRequirement
            {
                AuthResourceType = Authorization.AuthResourceType.Account,
                ResourceId = accountId,
                Scopes = Authorization.AuthScopes.Readable
            });
            try {
                if (!await WebAuthorizationHandler.FulFillAny(IdentityRepo, authorization, reqs)) { return HttpUnauthorized(); }
                Account account = await Repo.AccountManager.FindAccount(accountId);
                if (account.Book.Id != bookId) return HttpNotFound(new { error = "Account [" + accountId + "] cannot be found in Book [" + bookId + "]" });
                return new ObjectResult(account);
            }
            catch (TokenExtractionException) { return HttpUnauthorized(); }
            catch (PiggyBankDataNotFoundException) { return HttpUnauthorized(); }
            catch (PiggyBankBookException) { return HttpUnauthorized(); }
            catch (PiggyBankDataException e) { return HttpBadRequest(new { error = e.Message }); }
        }

        [HttpPut("{accountId}")]
        public async Task<IActionResult> Put(int userId, int bookId, int accountId, [FromBody] Account account, [FromHeader] string authorization)
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
                if (!await WebAuthorizationHandler.FulFillAny(IdentityRepo, authorization, reqs)) { return HttpUnauthorized(); }
                if (account == null) return HttpBadRequest(new { error = "Account object is missing" });
                if (account.Id != accountId) return HttpBadRequest(new { error = "Invalid Account.Id [" + account.Id + "]" });
                Account accountToUpdate = await Repo.AccountManager.FindAccount(accountId);
                if (accountToUpdate.Book.Id != bookId) return HttpNotFound(new { error = "Account [" + account.Id + "] not found in Book [" + bookId + "]" });
                await Repo.AccountManager.UpdateAccount(account);
                return new NoContentResult();
            }
            catch (TokenExtractionException) { return HttpUnauthorized(); }
            catch (PiggyBankDataNotFoundException) { return HttpUnauthorized(); }
            catch (PiggyBankBookException) { return HttpUnauthorized(); }
            catch (PiggyBankDataException e) { return HttpBadRequest(new { error = e.Message }); }
        }

        [HttpGet("{accountId}/detail")]
        public async Task<IActionResult> GetDetail(int userId, int bookId, int accountId, [FromHeader] string authorization)
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
            reqs.Add(new AuthorizationRequirement
            {
                AuthResourceType = Authorization.AuthResourceType.Account,
                ResourceId = accountId,
                Scopes = Authorization.AuthScopes.Readable
            });
            try
            {
                if (!await WebAuthorizationHandler.FulFillAny(IdentityRepo, authorization, reqs)) { return HttpUnauthorized(); }
                return new ObjectResult(await Repo.AccountManager.GetAccountDetail(accountId));
            }
            catch (TokenExtractionException) { return HttpUnauthorized(); }
            catch (PiggyBankDataNotFoundException) { return HttpUnauthorized(); }
            catch (PiggyBankBookException) { return HttpUnauthorized(); }
            catch (PiggyBankDataException e) { return HttpBadRequest(new { error = e.Message }); }
        }

        [HttpGet("{accountId}/transactions")]
        public async Task<IActionResult> GetTransactions(int userId, int bookId, int accountId, [FromHeader] string authorization)
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
            reqs.Add(new AuthorizationRequirement
            {
                AuthResourceType = Authorization.AuthResourceType.Account,
                ResourceId = accountId,
                Scopes = Authorization.AuthScopes.Readable
            });
            try
            {
                if (!await WebAuthorizationHandler.FulFillAny(IdentityRepo, authorization, reqs)) { return HttpUnauthorized(); }
                return new ObjectResult(await Repo.AccountManager.GetTransactions(accountId));
            }
            catch (TokenExtractionException) { return HttpUnauthorized(); }
            catch (PiggyBankDataNotFoundException) { return HttpUnauthorized(); }
            catch (PiggyBankBookException) { return HttpUnauthorized(); }
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
