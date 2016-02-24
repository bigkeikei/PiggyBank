using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

using SimpleIdentity.Models;

namespace PiggyBank.Controllers
{
    public class WebAuthorizationHandler
    {
        public string Token { get; set; }
        public string Signature { get; set; }

        public WebAuthorizationHandler(string authorization)
        {
            if (authorization == null || authorization.Length == 0) throw new TokenExtractionException("Authorization is missing");
            if (!authorization.StartsWith("Bearer ")) throw new TokenExtractionException("Invalid authorization");
            if (!authorization.Contains('.')) throw new TokenExtractionException("Invalid authorization");
            int pos = authorization.IndexOf('.');
            Token = authorization.Substring(7, pos - 7);
            Signature = (pos == authorization.Length ? null : authorization.Substring(pos + 1));
        }

        public async Task<bool> IsValid(ISimpleIdentityRepository repo, string method, string url, Stream body = null,  Dictionary<string, string> parameters = null)
        {
            string computedSignature = await repo.TokenManager.ComputeSignature(Token, method, url, body, parameters);
            return (computedSignature == null || Signature == computedSignature);
        }

        public async Task<bool> FulFill(ISimpleIdentityRepository repo, AuthorizationRequirement req)
        {
            return await req.Fulfill(repo, Token);
        }

        public async Task<bool> FulFillAny(ISimpleIdentityRepository repo, IEnumerable<AuthorizationRequirement> reqs)
        {
            return await AuthorizationRequirement.FulfillAny(repo, Token, reqs);
        }
    }

    public class TokenExtractionException : Exception
    {
        public TokenExtractionException(string message) : base(message) { }
    }
}
