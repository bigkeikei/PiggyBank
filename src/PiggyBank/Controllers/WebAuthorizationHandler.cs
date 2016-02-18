using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SimpleIdentity.Models;

namespace PiggyBank.Controllers
{
    public class WebAuthorizationHandler
    {
        private static string ExtractToken(string authorization)
        {
            if (authorization == null || authorization.Length == 0) throw new TokenExtractionException("Authorization is missing");
            if (!authorization.StartsWith("Bearer ")) throw new TokenExtractionException("Invalid authorization");
            return authorization.Substring(7);
        }

        public static async Task<bool> FulFill(ISimpleIdentityRepository repo, string authorization, AuthorizationRequirement req)
        {
            string token = ExtractToken(authorization);
            return await req.Fulfill(repo, token);
        }

        public static async Task<bool> FulFillAny(ISimpleIdentityRepository repo, string authorization, IEnumerable<AuthorizationRequirement> reqs)
        {
            string token = ExtractToken(authorization);
            return await AuthorizationRequirement.FulfillAny(repo, token, reqs);
        }
    }

    public class TokenExtractionException : Exception
    {
        public TokenExtractionException(string message) : base(message) { }
    }
}
