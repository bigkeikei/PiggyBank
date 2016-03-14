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
    [Route("api/[controller]")]
    public class TagsController : Controller
    {
        [FromServices]
        public IPiggyBankRepository Repo { get; set; }

        [FromServices]
        public ISimpleIdentityRepository IdentityRepo { get; set; }

        [HttpGet("{tagId}", Name = "GetTag")]
        public async Task<IActionResult> Get(int tagId, [FromHeader] string authorization)
        {
            try
            {
                Tag tag = await Repo.TagManager.FindTag(tagId);
                Book book = await Repo.BookManager.FindBook(tag.Book.Id);
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
                return new ObjectResult(tag);
            }
            catch (TokenExtractionException) { return HttpUnauthorized(); }
            catch (PiggyBankDataNotFoundException) { return HttpUnauthorized(); }
            catch (PiggyBankDataException e) { return HttpBadRequest(new { error = e.Message }); }
        }

        [HttpPut("{tagId}")]
        public async Task<IActionResult> Put(int tagId, [FromBody] Tag tag, [FromHeader] string authorization)
        {
            if (tag == null) return HttpBadRequest(new { error = "Tag object is missing" });
            if (tag.Id != tagId) return HttpBadRequest(new { error = "Invalid Tag.Id [" + tag.Id + "]" });
            try
            {
                Tag tagToUpdate = await Repo.TagManager.FindTag(tagId);
                Book book = await Repo.BookManager.FindBook(tag.Book.Id);
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
                    Scopes = Authorization.AuthScopes.Editable
                });
                WebAuthorizationHandler authHandler = new WebAuthorizationHandler(authorization);
                if (!await authHandler.IsValid(IdentityRepo, Request.Method, Request.Path, Request.Body)) { return HttpUnauthorized(); }
                if (!await authHandler.FulFillAny(IdentityRepo, reqs)) { return HttpUnauthorized(); }
                await Repo.TagManager.UpdateTag(tag);
                return new NoContentResult();
            }
            catch (TokenExtractionException) { return HttpUnauthorized(); }
            catch (PiggyBankDataNotFoundException) { return HttpUnauthorized(); }
            catch (PiggyBankDataException e) { return HttpBadRequest(new { error = e.Message }); }
        }
    }
}
