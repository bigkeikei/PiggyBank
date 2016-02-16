using System.Collections.Generic;
using Microsoft.AspNet.Mvc;

using PiggyBank.Models;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace PiggyBank.Controllers
{
    [Route("api")]
    public class UsersController : Controller
    {

        [FromServices]
        public IPiggyBankRepository Repo { get; set; }

        [HttpGet("[controller]")]
        public async Task<IEnumerable<User>> List()
        {
            return await Repo.UserManager.ListUsers();
        }

        [HttpGet("[controller]/{userId}", Name = "GetUser")]
        public async Task<IActionResult> Get(int userId, [FromHeader] string authorization)
        {
            try
            {
                return new ObjectResult(await Repo.UserManager.FindUser(userId));
            }
            catch (PiggyBankUserException) { return HttpUnauthorized(); }
            catch (PiggyBankDataNotFoundException) { return HttpUnauthorized(); }
            catch (PiggyBankDataException e) { return HttpBadRequest(new { error = e.Message }); }
        }

        [HttpGet("me")]
        public async Task<IActionResult> Get([FromHeader] string authorization)
        {
            try
            {
                string token = authorization.Substring(7);
                return new ObjectResult(await Repo.UserManager.FindUserByToken(token));
            }
            catch (PiggyBankUserException) { return HttpUnauthorized(); }
            catch (PiggyBankDataNotFoundException) { return HttpUnauthorized(); }
            catch (PiggyBankDataException e) { return HttpBadRequest(new { error = e.Message }); }
        }

        [HttpPost("[controller]")]
        public async Task<IActionResult> Post([FromBody]User user)
        {
            try
            {
                if (user == null) return HttpBadRequest(new { error = "User object missing"});
                User userCreated = await Repo.UserManager.CreateUser(user);
                return CreatedAtRoute("GetUser", new { controller = "users", userid = user.Id }, userCreated);
            }
            catch (PiggyBankDataException e) { return HttpBadRequest(new { error = e.Message }); }
        }

        [HttpPut("[controller]/{userId}")]
        public async Task<IActionResult> Put(int userId, [FromHeader] string authorization, [FromBody]User user)
        {
            try
            {
                if (user.Id != userId) return HttpUnauthorized();
                await Repo.UserManager.UpdateUser(user);
                return new NoContentResult();
            }
            catch (PiggyBankUserException) { return HttpUnauthorized(); }
            catch (PiggyBankDataNotFoundException) { return HttpUnauthorized(); }
            catch (PiggyBankDataException e) { return HttpBadRequest(new { error = e.Message }); }
        }
    }
}
