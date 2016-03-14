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
    public class TransactionsController : Controller
    {
        [FromServices]
        public IPiggyBankRepository Repo { get; set; }

        [FromServices]
        public ISimpleIdentityRepository IdentityRepo { get; set; }

        [HttpGet("{transactionId}", Name = "GetTransaction")]
        public async Task<IActionResult> Get(int transactionId, [FromHeader] string authorization)
        {
            try
            {
                Transaction transaction = await Repo.TransactionManager.FindTransaction(transactionId);
                Book book = await Repo.BookManager.FindBook(transaction.Book.Id);
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
                return new ObjectResult(transaction);
            }
            catch (TokenExtractionException) { return HttpUnauthorized(); }
            catch (PiggyBankBookException) { return HttpUnauthorized(); }
            catch (PiggyBankDataNotFoundException) { return HttpUnauthorized(); }
            catch (PiggyBankDataException e) { return HttpBadRequest(new { error = e.Message }); }
        }

        [HttpPut("{transactionId}")]
        public async Task<IActionResult> Put(int transactionId, [FromBody]Transaction transaction, [FromHeader] string authorization)
        {
            if (transaction == null) return HttpBadRequest(new { error = "Transaction object is missing" });
            if (transaction.Id != transactionId) return HttpBadRequest(new { error = "Invalid Transaction.Id [" + transaction.Id + "]" });

            try
            {
                List<AuthorizationRequirement> reqs = new List<AuthorizationRequirement>();
                Transaction transactionToUpdate = await Repo.TransactionManager.FindTransaction(transactionId);
                Book book = await Repo.BookManager.FindBook(transaction.Book.Id);
                reqs.Add(new AuthorizationRequirement
                {
                    AuthResourceType = Authorization.AuthResourceType.User,
                    ResourceId = book.UserId,
                    Scopes = Authorization.AuthScopes.Editable
                });
                reqs.Add(new AuthorizationRequirement
                {
                    AuthResourceType = Authorization.AuthResourceType.Book,
                    ResourceId = book.Id,
                    Scopes = Authorization.AuthScopes.Editable
                });
                WebAuthorizationHandler authHandler = new WebAuthorizationHandler(authorization);
                if (!await authHandler.IsValid(IdentityRepo, Request.Method, Request.Path, Request.Body)) { return HttpUnauthorized(); }
                if (!await authHandler.FulFillAny(IdentityRepo, reqs)) { return HttpUnauthorized(); }
                await Repo.TransactionManager.UpdateTransaction(transaction);
                return new NoContentResult();
            }
            catch (TokenExtractionException) { return HttpUnauthorized(); }
            catch (PiggyBankBookException) { return HttpUnauthorized(); }
            catch (PiggyBankDataNotFoundException) { return HttpUnauthorized(); }
            catch (PiggyBankDataException e) { return HttpBadRequest(new { error = e.Message }); }
        }

        [HttpPost("{transactionId}/tags")]
        public async Task<IActionResult> PostTag(int transactionId, [FromBody]Tag tag, [FromHeader] string authorization)
        {
            List<AuthorizationRequirement> reqs = new List<AuthorizationRequirement>();
            try
            {
                Transaction transaction = await Repo.TransactionManager.FindTransaction(transactionId);
                Book book = await Repo.BookManager.FindBook(transaction.Book.Id);
                reqs.Add(new AuthorizationRequirement
                {
                    AuthResourceType = Authorization.AuthResourceType.User,
                    ResourceId = book.UserId,
                    Scopes = Authorization.AuthScopes.Editable
                });
                reqs.Add(new AuthorizationRequirement
                {
                    AuthResourceType = Authorization.AuthResourceType.Book,
                    ResourceId = book.Id,
                    Scopes = Authorization.AuthScopes.Editable
                });
                WebAuthorizationHandler authHandler = new WebAuthorizationHandler(authorization);
                if (!await authHandler.IsValid(IdentityRepo, Request.Method, Request.Path, Request.Body)) { return HttpUnauthorized(); }
                if (!await authHandler.FulFillAny(IdentityRepo, reqs)) { return HttpUnauthorized(); }
                await Repo.TransactionManager.AddTag(transactionId, tag);
                return new NoContentResult();
            }
            catch (TokenExtractionException) { return HttpUnauthorized(); }
            catch (PiggyBankBookException) { return HttpUnauthorized(); }
            catch (PiggyBankDataNotFoundException) { return HttpUnauthorized(); }
            catch (PiggyBankDataException e) { return HttpBadRequest(new { error = e.Message }); }
        }

        [HttpDelete("{transactionId}/tags/{tagId}")]
        public async Task<IActionResult> DeleteTag(int transactionId, int tagId, [FromHeader] string authorization)
        {
            List<AuthorizationRequirement> reqs = new List<AuthorizationRequirement>();
            try
            {
                Transaction transaction = await Repo.TransactionManager.FindTransaction(transactionId);
                Book book = await Repo.BookManager.FindBook(transaction.Book.Id);
                reqs.Add(new AuthorizationRequirement
                {
                    AuthResourceType = Authorization.AuthResourceType.User,
                    ResourceId = book.UserId,
                    Scopes = Authorization.AuthScopes.Editable
                });
                reqs.Add(new AuthorizationRequirement
                {
                    AuthResourceType = Authorization.AuthResourceType.Book,
                    ResourceId = book.Id,
                    Scopes = Authorization.AuthScopes.Editable
                });
                WebAuthorizationHandler authHandler = new WebAuthorizationHandler(authorization);
                if (!await authHandler.IsValid(IdentityRepo, Request.Method, Request.Path)) { return HttpUnauthorized(); }
                if (!await authHandler.FulFillAny(IdentityRepo, reqs)) { return HttpUnauthorized(); }
                await Repo.TransactionManager.RemoveTag(transactionId, tagId);
                return new NoContentResult();
            }
            catch (TokenExtractionException) { return HttpUnauthorized(); }
            catch (PiggyBankBookException) { return HttpUnauthorized(); }
            catch (PiggyBankDataNotFoundException) { return HttpUnauthorized(); }
            catch (PiggyBankDataException e) { return HttpBadRequest(new { error = e.Message }); }
        }
    }
}
