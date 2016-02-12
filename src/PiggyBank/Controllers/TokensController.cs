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
        public async Task<IActionResult> GetChallenge(string userName)
        {
            try
            {
                User user = await Repo.UserManager.FindUserByName(userName);
                UserAuthentication auth = await Repo.UserManager.GenerateChallenge(user.Id);
                return new ObjectResult(new { Challenge = auth.Challenge });
            }
            catch (PiggyBankDataNotFoundException e) { return HttpNotFound(new { error = e.Message }); }
        }

        [HttpGet("token/{username}")]
        public async Task<IActionResult> GetToken(string userName, [FromQuery] string signature)
        {
            try
            {
                User user = await Repo.UserManager.FindUserByName(userName);
                return new ObjectResult(await Repo.UserManager.GenerateToken(user.Id, signature));

            }
            catch (PiggyBankAuthenticationTimeoutException e) { return HttpBadRequest(new { error = e.Message }); }
            catch (PiggyBankDataNotFoundException e) { return HttpNotFound(new { error = e.Message }); }
            catch (PiggyBankDataException e) { return HttpBadRequest(new { error = e.Message }); }
        }
    }
}
