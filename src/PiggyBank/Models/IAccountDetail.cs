using Newtonsoft.Json;


namespace PiggyBank.Models
{
    public interface IAccountDetail
    {
        [JsonIgnore]
        Account Account { get; }

        double Balance { get; }
        double BookBalance { get; }

    }
}
