using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;

using SimpleIdentity.Models;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace PiggyBank.Controllers
{
    [Route("api/[controller]")]
    public class TokensController : Controller
    {
        [FromServices]
        public ISimpleIdentityRepository IdentityRepo { get; set; }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]int? userId, [FromQuery]int? clientId, [FromQuery]string sign)
        {
            if (userId == null) { return HttpBadRequest(new { error = "userId is missing" }); }
            if (clientId == null) { return HttpBadRequest(new { error = "clientId is missing"}); }
            if (sign == null) { return HttpBadRequest(new { error = "sign is missing" }); }
            try
            {
                return new ObjectResult(await IdentityRepo.TokenManager.GenerateTokenBySignature(userId.Value, clientId.Value, sign));
            }
            catch (SimpleIdentityDataNotFoundException) { return HttpUnauthorized(); }
            catch (SimpleIdentityDataException) { return HttpUnauthorized(); }
        }
    }
}
