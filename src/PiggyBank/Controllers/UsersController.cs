using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using PiggyBank.Models;
using PiggyBank.Models.Data;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace PiggyBank.Controllers
{
    [Route("api")]
    public class UsersController : Controller
    {

        [FromServices]
        public IUserRepository Users { get; set; }

        [HttpGet("[controller]")]
        public IEnumerable<User> Get()
        {
            return Users.List();
        }

        [HttpGet("{name}", Name = "GetUser")]
        public IActionResult Get(string name)
        {
            return new ObjectResult(Users.Find(name));
        }

        [HttpPost("[controller]")]
        public IActionResult Post([FromBody]User user)
        {
            if (user == null)
            {
                return HttpBadRequest();
            }
            User userCreated = Users.Create(user);
            return CreatedAtRoute("GetUser", new { controller = "users", name = user.Name }, userCreated);
        }

        [HttpPut("{name}")]
        public IActionResult Put(string name, [FromBody]User user)
        {
            if (user == null || name != user.Name)
            {
                return HttpBadRequest();
            }
            User userToUpdate = Users.Find(name);
            if (userToUpdate == null || userToUpdate.Id != user.Id)
            {
                return HttpNotFound();
            }
            Users.Update(user);
            return new NoContentResult();
        }
    }
}
