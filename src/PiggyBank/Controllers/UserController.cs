using System.Collections.Generic;
using Microsoft.AspNet.Mvc;

using SimpleIdentity.Models;
using System.Threading.Tasks;
using System;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace PiggyBank.Controllers
{
    [Route("api")]
    public class UsersController : Controller
    {

        [FromServices]
        public ISimpleIdentityRepository IdentityRepo { get; set; }

        [HttpGet("[controller]/{userId}", Name = "GetUser")]
        public async Task<IActionResult> Get(int userId, [FromHeader] string authorization)
        {
            AuthorizationRequirement req = new AuthorizationRequirement
            {
                AuthResourceType = Authorization.AuthResourceType.User,
                ResourceId = userId,
                Scopes = Authorization.AuthScopes.Readable
            };
            try
            {
                if (!await WebAuthorizationHandler.FulFill(IdentityRepo, authorization, req)) { return HttpUnauthorized(); }
                return new ObjectResult(await IdentityRepo.UserManager.FindUser(userId));
            }
            catch (TokenExtractionException) { return HttpUnauthorized(); }
            catch (SimpleIdentityDataNotFoundException) { return HttpUnauthorized(); }
        }

        [HttpGet("me")]
        public async Task<IActionResult> Get([FromHeader] string authorization)
        {
            try
            {
                string token = authorization.Substring(7);
                return new ObjectResult(await IdentityRepo.UserManager.FindUserByToken(token));
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
            AuthorizationRequirement req = new AuthorizationRequirement
            {
                AuthResourceType = Authorization.AuthResourceType.User,
                ResourceId = userId,
                Scopes = Authorization.AuthScopes.Editable
            };
            try
            {
                if (!await WebAuthorizationHandler.FulFill(IdentityRepo, authorization, req)) { return HttpUnauthorized(); }
                if (user.Id != userId) return HttpUnauthorized();
                await IdentityRepo.UserManager.UpdateUser(user);
                return new NoContentResult();
            }
            catch (SimpleIdentityUserException) { return HttpUnauthorized(); }
            catch (SimpleIdentityDataNotFoundException) { return HttpUnauthorized(); }
            catch (SimpleIdentityDataException e) { return HttpBadRequest(new { error = e.Message }); }
        }

    }
}
