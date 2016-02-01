using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;

using PiggyBank.Models;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace PiggyBank.Controllers
{
    [Route("api")]
    public class TokensController : Controller
    {
        [FromServices]
        public IPiggyBankRepository Repo { get; set; }

        [HttpGet("challenge/{username}")]
        public IActionResult GetChallenge(string userName)
        {
            try
            {
                User user = Repo.UserManager.FindUserByName(userName);
                UserAuthentication auth = Repo.UserManager.GenerateChallenge(user.Id);
                return new ObjectResult(new { Challenge = auth.Challenge });
            }
            catch (PiggyBankDataNotFoundException e) { return HttpNotFound(new { error = e.Message }); }
        }

        [HttpGet("token/{username}")]
        public IActionResult GetToken(string userName, [FromQuery] string signature)
        {
            try
            {
                User user = Repo.UserManager.FindUserByName(userName);
                return new ObjectResult(Repo.UserManager.GenerateToken(user.Id, signature));

            }
            catch (PiggyBankAuthenticationTimeoutException e) { return HttpBadRequest(new { error = e.Message }); }
            catch (PiggyBankDataNotFoundException e) { return HttpNotFound(new { error = e.Message }); }
            catch (PiggyBankDataException e) { return HttpBadRequest(new { error = e.Message }); }
        }
    }
}
