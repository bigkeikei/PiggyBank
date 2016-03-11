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
            AuthorizationRequirement req = new AuthorizationRequirement
            {
                AuthResourceType = Authorization.AuthResourceType.User,
                ResourceId = userId,
                Scopes = Authorization.AuthScopes.Editable
            };
            try
            {
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

    }
}
