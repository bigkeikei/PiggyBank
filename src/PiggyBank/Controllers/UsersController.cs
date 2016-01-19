using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using PiggyBank.Models;
using PiggyBank.Models.Data;
using Microsoft.AspNet.Authorization;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace PiggyBank.Controllers
{
    [Route("api")]
    public class UsersController : Controller
    {

        [FromServices]
        public IPiggyBankRepository Repo { get; set; }

        [HttpGet("[controller]")]
        public IEnumerable<User> Get()
        {
            return Repo.ListUsers();
        }

        [HttpGet("{name}", Name = "GetUser")]
        public IActionResult Get(string name, [FromHeader] string authorization)
        {
            if (!IsAuthorized(name, authorization)) return HttpUnauthorized();
            return new ObjectResult(Repo.FindUser(name));
        }

        [HttpPost("[controller]")]
        public IActionResult Post([FromBody]User user)
        {
            if (user == null)
            {
                return HttpBadRequest();
            }
            User userCreated = Repo.CreateUser(user);
            return CreatedAtRoute("GetUser", new { controller = "users", name = user.Name }, userCreated);
        }

        [HttpPut("{name}")]
        public IActionResult Put(string name, [FromHeader] string authorization, [FromBody]User user)
        {
            if (!IsAuthorized(name, authorization)) return HttpUnauthorized();
            if (user == null || name != user.Name)
            {
                return HttpNotFound(new { Error = "user[" + name + "] not found"});
            }
            User userToUpdate = Repo.FindUser(name);
            if (userToUpdate == null || userToUpdate.Id != user.Id)
            {
                return HttpBadRequest(new { Error = "user.Id [" + user.Id +"] does not match" });
            }
            Repo.UpdateUser(user);
            return new NoContentResult();
        }

        [HttpGet("{name}/token")]
        public IActionResult GetToken(string name, [FromQuery] string signature)
        {
            User user = Repo.FindUser(name);
            if (user == null)
            {
                return HttpNotFound(new { Error = "user[" + name + "] not found" });
            }
            if (signature != user.Authentication.Signature)
            {
                return HttpBadRequest(new { Error = "signature[" + signature + "] does not match" });
            }
            return new ObjectResult(user.Authentication);
        }

        [HttpGet("{name}/challenge")]
        public IActionResult GetChallenge(string name)
        {
            User user = Repo.FindUser(name);
            if (user == null)
            {
                return HttpNotFound(new { error = "user[" + name + "] not found" });
            }
            UserAuthentication auth = Repo.GenerateAuthentication(user);
            return new ObjectResult(new { Challenge = auth.Challenge });
        }

        private bool IsAuthorized(string name, string authorization)
        {
            // bypass for development
            return true;

            User user = Repo.FindUser(name);
            return !(authorization != null && authorization != "Bearer " + user.Authentication.AccessToken);
        }
    }
}
