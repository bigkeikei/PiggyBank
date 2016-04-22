using System.Collections.Generic;
using Microsoft.AspNet.Mvc;

using SimpleIdentity.Models;
using PiggyBank.Models;
using System.Threading.Tasks;
using System;
using System.Linq;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace PiggyBank.Controllers
{
    [Route("api")]
    public class UsersController : Controller
    {
        [FromServices]
        public ISimpleIdentityRepository IdentityRepo { get; set; }

        [FromServices]
        public IPiggyBankRepository Repo { get; set; }

        [HttpGet("[controller]/{userId}", Name = "GetUser")]
        public async Task<IActionResult> Get(int userId, [FromHeader] string authorization)
        {
            try
            {
                AuthorizationRequirement req = new AuthorizationRequirement
                {
                    AuthResourceType = Authorization.AuthResourceType.User,
                    ResourceId = userId,
                    Scopes = Authorization.AuthScopes.Readable
                };
                WebAuthorizationHandler authHandler = new WebAuthorizationHandler(authorization);
                if (!await authHandler.IsValid(IdentityRepo, Request.Method, Request.Path)) { return HttpUnauthorized(); }
                if (!await authHandler.FulFill(IdentityRepo, req)) { return HttpUnauthorized(); }
                return new ObjectResult(await IdentityRepo.UserManager.FindUser(userId));
            }
            catch (TokenExtractionException) { return HttpUnauthorized(); }
            catch (SimpleIdentityDataNotFoundException) { return HttpUnauthorized(); }
        }

        [HttpGet("[controller]/{userId}/nonce")]
        public async Task<IActionResult> GetNonce(int userId)
        {
            try
            {
                return new ObjectResult(await IdentityRepo.UserManager.GenerateNonce(userId));
            }
            catch (SimpleIdentityDataNotFoundException ex) { return HttpBadRequest(new { error = ex.Message }); }
        }

        [HttpGet("me")]
        public async Task<IActionResult> Get([FromHeader] string authorization)
        {
            try
            {
                WebAuthorizationHandler authHandler = new WebAuthorizationHandler(authorization);
                if (!await authHandler.IsValid(IdentityRepo, Request.Method, Request.Path)) { return HttpUnauthorized(); }
                return new ObjectResult(await IdentityRepo.UserManager.FindUserByToken(authHandler.Token));
            }
            catch (SimpleIdentityUserException) { return HttpUnauthorized(); }
            catch (SimpleIdentityDataNotFoundException) { return HttpUnauthorized(); }
            catch (SimpleIdentityDataException e) { return HttpBadRequest(new { error = e.Message }); }
        }

        [HttpPost("[controller]")]
        public async Task<IActionResult> Post([FromBody]User user)
        {
            try
            {
                if (user == null) return HttpBadRequest(new { error = "User object missing" });
                User userCreated = await IdentityRepo.UserManager.CreateUser(user);
                return CreatedAtRoute("GetUser", new { controller = "users", userid = user.Id }, userCreated);
            }
            catch (SimpleIdentityDataException e) { return HttpBadRequest(new { error = e.Message }); }
        }

        [HttpPut("[controller]/{userId}")]
        public async Task<IActionResult> Put(int userId, [FromHeader] string authorization, [FromBody]User user)
        {
            try
            {
                AuthorizationRequirement req = new AuthorizationRequirement
                {
                    AuthResourceType = Authorization.AuthResourceType.User,
                    ResourceId = userId,
                    Scopes = Authorization.AuthScopes.Editable
                };
                WebAuthorizationHandler authHandler = new WebAuthorizationHandler(authorization);
                if (!await authHandler.IsValid(IdentityRepo, Request.Method, Request.Path, Request.Body)) { return HttpUnauthorized(); }
                if (!await authHandler.FulFill(IdentityRepo, req)) { return HttpUnauthorized(); }
                if (user.Id != userId) return HttpUnauthorized();
                await IdentityRepo.UserManager.UpdateUser(user);
                return new NoContentResult();
            }
            catch (SimpleIdentityUserException) { return HttpUnauthorized(); }
            catch (SimpleIdentityDataNotFoundException) { return HttpUnauthorized(); }
            catch (SimpleIdentityDataException e) { return HttpBadRequest(new { error = e.Message }); }
        }

        [HttpGet("[controller]/{userId}/books")]
        public async Task<IActionResult> GetBooks(int userId, [FromHeader] string authorization)
        {
            try
            {
                AuthorizationRequirement req = new AuthorizationRequirement
                {
                    AuthResourceType = Authorization.AuthResourceType.User,
                    ResourceId = userId,
                    Scopes = Authorization.AuthScopes.Readable
                };
                WebAuthorizationHandler authHandler = new WebAuthorizationHandler(authorization);
                if (!await authHandler.IsValid(IdentityRepo, Request.Method, Request.Path)) { return HttpUnauthorized(); }
                if (!await authHandler.FulFill(IdentityRepo, req)) { return HttpUnauthorized(); }

                List<Book> readableBooks = new List<Book>();
                IEnumerable<Book> books = await Repo.BookManager.ListBooks(userId);
                foreach (Book book in books)
                {
                    List<AuthorizationRequirement> bookReqs = new List<AuthorizationRequirement>();
                    bookReqs.Add(new AuthorizationRequirement
                    {
                        AuthResourceType = Authorization.AuthResourceType.User,
                        ResourceId = userId,
                        Scopes = Authorization.AuthScopes.Full
                    });
                    bookReqs.Add(new AuthorizationRequirement
                    {
                        AuthResourceType = Authorization.AuthResourceType.Book,
                        ResourceId = book.Id,
                        Scopes = Authorization.AuthScopes.Readable
                    });
                    if (await authHandler.FulFillAny(IdentityRepo, bookReqs)) { readableBooks.Add(book); }
                }
                return new ObjectResult(readableBooks.AsEnumerable());
            }
            catch (TokenExtractionException) { return HttpUnauthorized(); }
            catch (PiggyBankDataNotFoundException) { return HttpUnauthorized(); }
        }

        [HttpPost("[controller]/{userId}/books")]
        public async Task<IActionResult> PostBook(int userId, [FromBody] Book book, [FromHeader] string authorization)
        {
            if (book.UserId != userId) { return HttpUnauthorized(); }
            try
            {
                AuthorizationRequirement req = new AuthorizationRequirement
                {
                    AuthResourceType = Authorization.AuthResourceType.User,
                    ResourceId = userId,
                    Scopes = Authorization.AuthScopes.Editable
                };
                WebAuthorizationHandler authHandler = new WebAuthorizationHandler(authorization);
                if (!await authHandler.IsValid(IdentityRepo, Request.Method, Request.Path, Request.Body)) { return HttpUnauthorized(); }
                if (!await authHandler.FulFill(IdentityRepo, req)) { return HttpUnauthorized(); }
                Book bookCreated = await Repo.BookManager.CreateBook(book);
                return CreatedAtRoute("GetBook", new { controller = "books", bookId = bookCreated.Id }, bookCreated);
            }
            catch (TokenExtractionException) { return HttpUnauthorized(); }
            catch (PiggyBankDataNotFoundException) { return HttpUnauthorized(); }
            catch (PiggyBankDataException e) { return HttpBadRequest(new { error = e.Message }); }
        }

        [HttpGet("[controller]/{userId}/accounts")]
        public async Task<ActionResult> GetAccounts(int userId, [FromHeader] string authorization)
        {
            try
            {
                AuthorizationRequirement req = new AuthorizationRequirement
                {
                    AuthResourceType = Authorization.AuthResourceType.User,
                    ResourceId = userId,
                    Scopes = Authorization.AuthScopes.Readable
                };
                WebAuthorizationHandler authHandler = new WebAuthorizationHandler(authorization);
                if (!await authHandler.IsValid(IdentityRepo, Request.Method, Request.Path)) { return HttpUnauthorized(); }
                if (!await authHandler.FulFill(IdentityRepo, req)) { return HttpUnauthorized(); }
                IEnumerable<Account> accounts = await Repo.AccountManager.ListAccounts(userId);
                List<Account> readableAccounts = new List<Account>();
                foreach (Account account in accounts)
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
                return new ObjectResult(readableAccounts.AsEnumerable());
            }
            catch (TokenExtractionException) { return HttpUnauthorized(); }
        }
    }
}
