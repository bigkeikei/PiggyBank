using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;

using SimpleIdentity.Models;
using PiggyBank.Models;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace PiggyBank.Controllers
{
    [Route("api/[controller]")]
    public class AccountsController : Controller
    {
        [FromServices]
        public IPiggyBankRepository Repo { get; set; }

        [FromServices]
        public ISimpleIdentityRepository IdentityRepo { get; set; }

        [HttpGet("{accountId}", Name = "GetAccount")]
        public async Task<IActionResult> Get(int accountId, [FromHeader] string authorization)
        {
            try {
                Account account = await Repo.AccountManager.FindAccount(accountId,true);
                List<AuthorizationRequirement> reqs = new List<AuthorizationRequirement>();
                reqs.Add(new AuthorizationRequirement
                {
                    AuthResourceType = Authorization.AuthResourceType.User,
                    ResourceId = account.Book.UserId,
                    Scopes = Authorization.AuthScopes.Full
                });
                reqs.Add(new AuthorizationRequirement
                {
                    AuthResourceType = Authorization.AuthResourceType.Book,
                    ResourceId = account.Book.Id,
                    Scopes = Authorization.AuthScopes.Readable
                });
                reqs.Add(new AuthorizationRequirement
                {
                    AuthResourceType = Authorization.AuthResourceType.Account,
                    ResourceId = accountId,
                    Scopes = Authorization.AuthScopes.Readable
                });
                WebAuthorizationHandler authHandler = new WebAuthorizationHandler(authorization);
                if (!await authHandler.IsValid(IdentityRepo, Request.Method, Request.Path)) { return HttpUnauthorized(); }
                if (!await authHandler.FulFillAny(IdentityRepo, reqs)) { return HttpUnauthorized(); }
                return new ObjectResult(account);
            }
            catch (TokenExtractionException) { return HttpUnauthorized(); }
            catch (PiggyBankDataNotFoundException) { return HttpUnauthorized(); }
            catch (PiggyBankDataException e) { return HttpBadRequest(new { error = e.Message }); }
        }

        [HttpPut("{accountId}")]
        public async Task<IActionResult> Put(int accountId, [FromBody] Account account, [FromHeader] string authorization)
        {
            if (account == null) return HttpBadRequest(new { error = "Account object is missing" });
            if (account.Id != accountId) return HttpBadRequest(new { error = "Invalid Account.Id [" + account.Id + "]" });
            try
            {
                Account accountToUpdate = await Repo.AccountManager.FindAccount(accountId, true);
                List<AuthorizationRequirement> reqs = new List<AuthorizationRequirement>();
                reqs.Add(new AuthorizationRequirement
                {
                    AuthResourceType = Authorization.AuthResourceType.User,
                    ResourceId = accountToUpdate.Book.UserId,
                    Scopes = Authorization.AuthScopes.Full
                });
                reqs.Add(new AuthorizationRequirement
                {
                    AuthResourceType = Authorization.AuthResourceType.Book,
                    ResourceId = accountToUpdate.Book.Id,
                    Scopes = Authorization.AuthScopes.Editable
                });
                WebAuthorizationHandler authHandler = new WebAuthorizationHandler(authorization);
                if (!await authHandler.IsValid(IdentityRepo, Request.Method, Request.Path, Request.Body)) { return HttpUnauthorized(); }
                if (!await authHandler.FulFillAny(IdentityRepo, reqs)) { return HttpUnauthorized(); }
                await Repo.AccountManager.UpdateAccount(account);
                return new NoContentResult();
            }
            catch (TokenExtractionException) { return HttpUnauthorized(); }
            catch (PiggyBankDataNotFoundException) { return HttpUnauthorized(); }
            catch (PiggyBankDataException e) { return HttpBadRequest(new { error = e.Message }); }
        }

        [HttpGet("{accountId}/detail")]
        public async Task<IActionResult> GetDetail(int accountId, [FromHeader] string authorization)
        {
            try
            {
                Account account = await Repo.AccountManager.FindAccount(accountId, true);
                List<AuthorizationRequirement> reqs = new List<AuthorizationRequirement>();
                reqs.Add(new AuthorizationRequirement
                {
                    AuthResourceType = Authorization.AuthResourceType.User,
                    ResourceId = account.Book.UserId,
                    Scopes = Authorization.AuthScopes.Full
                });
                reqs.Add(new AuthorizationRequirement
                {
                    AuthResourceType = Authorization.AuthResourceType.Book,
                    ResourceId = account.Book.Id,
                    Scopes = Authorization.AuthScopes.Readable
                });
                reqs.Add(new AuthorizationRequirement
                {
                    AuthResourceType = Authorization.AuthResourceType.Account,
                    ResourceId = accountId,
                    Scopes = Authorization.AuthScopes.Readable
                });
                WebAuthorizationHandler authHandler = new WebAuthorizationHandler(authorization);
                if (!await authHandler.IsValid(IdentityRepo, Request.Method, Request.Path)) { return HttpUnauthorized(); }
                if (!await authHandler.FulFillAny(IdentityRepo, reqs)) { return HttpUnauthorized(); }
                return new ObjectResult(await Repo.AccountManager.GetAccountDetail(accountId));
            }
            catch (TokenExtractionException) { return HttpUnauthorized(); }
            catch (PiggyBankDataNotFoundException) { return HttpUnauthorized(); }
            catch (PiggyBankDataException e) { return HttpBadRequest(new { error = e.Message }); }
        }

        [HttpGet("{accountId}/transactions")]
        public async Task<IActionResult> GetTransactions(int accountId, [FromQuery] DateTime? periodStart, [FromQuery] DateTime? periodEnd, [FromQuery] int? noOfRecords, [FromHeader] string authorization)
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            if (periodStart != null) { args.Add("periodStart", Request.Query["periodStart"]); }
            if (periodEnd != null) { args.Add("periodEnd", Request.Query["periodEnd"]); }
            if (noOfRecords != null) { args.Add("noOfRecords", Request.Query["noOfRecords"]); }

            try
            {
                Account account = await Repo.AccountManager.FindAccount(accountId, true);
                List<AuthorizationRequirement> reqs = new List<AuthorizationRequirement>();
                reqs.Add(new AuthorizationRequirement
                {
                    AuthResourceType = Authorization.AuthResourceType.User,
                    ResourceId = account.Book.UserId,
                    Scopes = Authorization.AuthScopes.Full
                });
                reqs.Add(new AuthorizationRequirement
                {
                    AuthResourceType = Authorization.AuthResourceType.Book,
                    ResourceId = account.Book.Id,
                    Scopes = Authorization.AuthScopes.Readable
                });
                reqs.Add(new AuthorizationRequirement
                {
                    AuthResourceType = Authorization.AuthResourceType.Account,
                    ResourceId = accountId,
                    Scopes = Authorization.AuthScopes.Readable
                });
                WebAuthorizationHandler authHandler = new WebAuthorizationHandler(authorization);
                if (!await authHandler.IsValid(IdentityRepo, Request.Method, Request.Path, parameters: args)) { return HttpUnauthorized(); }
                if (!await authHandler.FulFillAny(IdentityRepo, reqs)) { return HttpUnauthorized(); }
                return new ObjectResult(await Repo.AccountManager.GetTransactions(accountId, periodStart, periodEnd, noOfRecords));
            }
            catch (TokenExtractionException) { return HttpUnauthorized(); }
            catch (PiggyBankDataNotFoundException) { return HttpUnauthorized(); }
            catch (PiggyBankDataException e) { return HttpBadRequest(new { error = e.Message }); }
        }

        [HttpGet("{accountId}/transactions/count")]
        public async Task<IActionResult> GetTransactionCount(int accountId, [FromQuery] DateTime? periodStart, [FromQuery] DateTime? periodEnd, [FromHeader] string authorization)
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            if (periodStart != null) { args.Add("periodStart", Request.Query["periodStart"]); }
            if (periodEnd != null) { args.Add("periodEnd", Request.Query["periodEnd"]); }

            try
            {
                Account account = await Repo.AccountManager.FindAccount(accountId, true);

                List<AuthorizationRequirement> reqs = new List<AuthorizationRequirement>();
                reqs.Add(new AuthorizationRequirement
                {
                    AuthResourceType = Authorization.AuthResourceType.User,
                    ResourceId = account.Book.UserId,
                    Scopes = Authorization.AuthScopes.Full
                });
                reqs.Add(new AuthorizationRequirement
                {
                    AuthResourceType = Authorization.AuthResourceType.Book,
                    ResourceId = account.Book.Id,
                    Scopes = Authorization.AuthScopes.Readable
                });
                reqs.Add(new AuthorizationRequirement
                {
                    AuthResourceType = Authorization.AuthResourceType.Account,
                    ResourceId = accountId,
                    Scopes = Authorization.AuthScopes.Readable
                });
                WebAuthorizationHandler authHandler = new WebAuthorizationHandler(authorization);
                if (!await authHandler.IsValid(IdentityRepo, Request.Method, Request.Path, parameters: args)) { return HttpUnauthorized(); }
                if (!await authHandler.FulFillAny(IdentityRepo, reqs)) { return HttpUnauthorized(); }
                return new ObjectResult(await Repo.AccountManager.GetTransactionCount(accountId, periodStart, periodEnd));
            }
            catch (TokenExtractionException) { return HttpUnauthorized(); }
            catch (PiggyBankDataNotFoundException) { return HttpUnauthorized(); }
            catch (PiggyBankDataException e) { return HttpBadRequest(new { error = e.Message }); }
        }
    }
}
