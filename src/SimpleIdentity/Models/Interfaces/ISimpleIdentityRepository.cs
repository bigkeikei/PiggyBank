namespace SimpleIdentity.Models
{
    public interface ISimpleIdentityRepository
    {
        IUserManager UserManager { get; }
        IAuthorizationManager AuthorizationManager { get; }
        ITokenManager TokenManager { get; }
    }
}