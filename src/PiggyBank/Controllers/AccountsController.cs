﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;

using PiggyBank.Models;
using PiggyBank.Models.Data;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace PiggyBank.Controllers
{
    [Route("api/users/{userId}/books/{bookId}/[controller]")]
    public class AccountsController : Controller
    {
        [FromServices]
        public IPiggyBankRepository Repo { get; set; }

        [HttpGet]
        public IActionResult List(int userId, int bookId, [FromHeader] string authorization)
        {
            try
            {
                return new ObjectResult(GetBook(userId, bookId, authorization).Accounts);
            }
            catch (PiggyBankDataNotFoundException e) { return HttpUnauthorized(); }
            catch (PiggyBankUserException e) { return HttpUnauthorized(); }
            catch (PiggyBankBookException e) { return HttpUnauthorized(); }
        }

        [HttpPost]
        public IActionResult Post(int userId, int bookId, [FromBody] Account account, [FromHeader] string authorization)
        {
            try
            {
                Book book = GetBook(userId, bookId, authorization);
                Account accountCreated = Repo.AccountManager.CreateAccount(book, account);
                return CreatedAtRoute("GetAccount", new { controller = "accounts", userId = userId, bookId = bookId, accountId = accountCreated.Id }, accountCreated);
            }
            catch (PiggyBankDataNotFoundException e) { return HttpUnauthorized(); }
            catch (PiggyBankUserException e) { return HttpUnauthorized(); }
            catch (PiggyBankBookException e) { return HttpUnauthorized(); }
            catch (PiggyBankDataException e) { return HttpBadRequest(new { error = e.Message }); }
        }

        [HttpGet("{accountId}", Name = "GetAccount")]
        public IActionResult Get(int userId, int bookId, int accountId, [FromHeader] string authorization)
        {
            try {
                Book book = GetBook(userId, bookId, authorization);
                Account account = Repo.AccountManager.FindAccount(accountId);
                if (account.Book.Id != book.Id) return HttpNotFound(new { error = "Account [" + accountId + "] cannot be found in Book [" + bookId + "]" });
                return new ObjectResult(account);
            }
            catch (PiggyBankDataNotFoundException e) { return HttpUnauthorized(); }
            catch (PiggyBankUserException e) { return HttpUnauthorized(); }
            catch (PiggyBankBookException e) { return HttpUnauthorized(); }
            catch (PiggyBankDataException e) { return HttpBadRequest(new { error = e.Message }); }
        }

        [HttpPut("{accountId}")]
        public IActionResult Put(int userId, int bookId, int accountId, [FromBody] Account account, [FromHeader] string authorization)
        {
            try
            {
                if (account == null) return HttpBadRequest(new { error = "Account object is missing" });
                if (account.Id != accountId) return HttpBadRequest(new { error = "Invalid Account.Id [" + account.Id + "]" });
                Book book = GetBook(userId, bookId, authorization);
                Account accountToUpdate = Repo.AccountManager.FindAccount(accountId);
                if (accountToUpdate.Book.Id != book.Id) return HttpNotFound(new { error = "Account [" + account.Id + "] not found in Book [" + bookId + "]" });
                Repo.AccountManager.UpdateAccount(account);
                return new NoContentResult();
            }
            catch (PiggyBankDataNotFoundException e) { return HttpUnauthorized(); }
            catch (PiggyBankUserException e) { return HttpUnauthorized(); }
            catch (PiggyBankBookException e) { return HttpUnauthorized(); }
            catch (PiggyBankDataException e) { return HttpBadRequest(new { error = e.Message }); }
        }

        [HttpGet("{accountId}/detail")]
        public IActionResult GetDetail(int userId, int bookId, int accountId, [FromHeader] string authorization)
        {
            try
            {
                Book book = GetBook(userId, bookId, authorization);
                return new ObjectResult(Repo.AccountManager.GetAccountDetail(accountId));
            }
            catch (PiggyBankDataNotFoundException e) { return HttpUnauthorized(); }
            catch (PiggyBankUserException e) { return HttpUnauthorized(); }
            catch (PiggyBankBookException e) { return HttpUnauthorized(); }
            catch (PiggyBankDataException e) { return HttpBadRequest(new { error = e.Message }); }
        }

        private Book GetBook(int userId, int bookId, string authorization)
        {
            User user = TokenRequirement.Fulfill(Repo, userId, authorization);
            Book book = Repo.BookManager.FindBook(bookId);
            if (book.User.Id != userId) throw new PiggyBankBookException("Book [" + bookId + "] not found in User [" + userId + "]");
            return book;
        }
    }
    
}
