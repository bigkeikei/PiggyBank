namespace SimpleIdentity.Models
{
    public interface ISimpleIdentityRepository
    {
        IUserManager UserManager { get; }
    }
}