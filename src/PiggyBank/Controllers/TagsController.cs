using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;

using PiggyBank.Models;
using SimpleIdentity.Models;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace PiggyBank.Controllers
{
    [Route("api/users/{userId}/books/{bookId}/[controller]")]
    public class TagsController : Controller
    {
        [FromServices]
        public IPiggyBankRepository Repo { get; set; }

        [FromServices]
        public ISimpleIdentityRepository IdentityRepo { get; set; }

        [HttpGet("{tagId}", Name = "GetTag")]
        public async Task<IActionResult> Get(int userId, int bookId, int tagId, [FromHeader] string authorization)
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
                await GetBook(userId, bookId);
                return new ObjectResult(await Repo.TagManager.FindTag(tagId));
            }
            catch (TokenExtractionException) { return HttpUnauthorized(); }
            catch (PiggyBankDataNotFoundException) { return HttpUnauthorized(); }
            catch (PiggyBankDataException e) { return HttpBadRequest(new { error = e.Message }); }
        }

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
                WebAuthorizationHandler authHandler = new WebAuthorizationHandler(authorization);
                if (!await authHandler.IsValid(IdentityRepo, Request.Method, Request.Path)) { return HttpUnauthorized(); }
                if (!await authHandler.FulFillAny(IdentityRepo, reqs)) { return HttpUnauthorized(); }
                await GetBook(userId, bookId);
                return new ObjectResult(await Repo.TagManager.ListTags(bookId));
            }
            catch (TokenExtractionException) { return HttpUnauthorized(); }
            catch (PiggyBankDataException e) { return HttpBadRequest(new { error = e.Message }); }
        }

        [HttpPost]
        public async Task<IActionResult> Post(int userId, int bookId, [FromBody] Tag tag, [FromHeader] string authorization)
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
                Tag tagCreated = await Repo.TagManager.CreateTag(book, tag);
                return CreatedAtRoute("GetTag", new { controller = "tags", userId = userId, bookId = bookId, tagId = tagCreated.Id }, tagCreated);
            }
            catch (TokenExtractionException) { return HttpUnauthorized(); }
            catch (PiggyBankDataNotFoundException) { return HttpUnauthorized(); }
            catch (PiggyBankBookException) { return HttpUnauthorized(); }
            catch (PiggyBankDataException e) { return HttpBadRequest(new { error = e.Message }); }
        }

        [HttpPut("{tagId}")]
        public async Task<IActionResult> Put(int userId, int bookId, int tagId, [FromBody] Tag tag, [FromHeader] string authorization)
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
                if (tag == null) return HttpBadRequest(new { error = "Tag object is missing" });
                if (tag.Id != tagId) return HttpBadRequest(new { error = "Invalid Tag.Id [" + tag.Id + "]" });
                await GetBook(userId, bookId);
                Tag tagToUpdate = await Repo.TagManager.FindTag(tagId);
                if (tagToUpdate.Book.Id != bookId) return HttpBadRequest(new { error = "Tag [" + tagId + "] cannot be found in Book [" + bookId + "]" });
                await Repo.TagManager.UpdateTag(tag);
                return new NoContentResult();
                
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
