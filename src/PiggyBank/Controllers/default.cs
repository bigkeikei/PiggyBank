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
    public class Default : Controller
    {
        [FromServices]
        public IPiggyBankRepository Repo { get; set; }

        [HttpGet]
        public string Get()
        {
            return "Welcome to PiggyBank API";
        }

        [HttpGet("play")]
        public async Task<string> Play()
        {
            await Repo.BookManager.CloseBook(5,DateTime.Now);
            return "Play!";
        }
    }
}
