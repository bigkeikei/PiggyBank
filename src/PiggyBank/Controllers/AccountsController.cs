using System;
using System.Collections.Generic;
using Microsoft.AspNet.Mvc;
using PiggyBank.Models;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace PiggyBank.Controllers
{
    [Route("api/[controller]")]
    public class AccountsController : Controller
    {
        [FromServices]
        public IAccountRepository AccountRepository { get; set; }

        [HttpGet]
        public IEnumerable<Account> Get()
        {
            return AccountRepository.List();
        }

        [HttpGet("{id}", Name = "GetAccount")]
        public IActionResult Get(int id)
        {
            return new ObjectResult(AccountRepository.Find(id));
        }

        [HttpPost]
        public IActionResult Post([FromBody]Account account)
        {
            if (account == null)
            {
                return HttpBadRequest();
            }
            var accountCreated = AccountRepository.Create(account);
            return CreatedAtRoute("GetAccount", new { controller = "account", id = accountCreated.Id }, accountCreated);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]Account account)
        {
            if (id != account.Id)
            {
                return HttpBadRequest();
            }
            if (AccountRepository.Find(account.Id) == null)
            {
                return HttpNotFound();
            }
            AccountRepository.Update(account);
            return new NoContentResult();
        }
    }
}
