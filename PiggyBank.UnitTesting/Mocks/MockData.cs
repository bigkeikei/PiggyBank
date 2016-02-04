using System.Collections.Generic;

using PiggyBank.Models;

namespace PiggyBank.UnitTesting.Mocks
{
    public class MockData
    {
        public MockData()
        {
            Users = new List<User>();
            Books = new List<Book>();
            Accounts = new List<Account>();
            Transactions = new List<Transaction>();
        }

        public List<User> Users { get; set; }
        public List<Book> Books { get; set; }
        public List<Account> Accounts { get; set; }
        public List<Transaction> Transactions { get; set; }

        public static MockData Seed()
        {
            MockData data = new MockData();
            data.Users.Add(new User { Id = 1, Name = "Happy Cat", Email = "cat@happy.com", IsActive = true });
            data.Users.Add(new User { Id = 2, Name = "Skiny Pig", Email = "pig@skiny.com", IsActive = true });
            data.Users.Add(new User { Id = 3, Name = "Silly Dog", Email = "dog@silly.com", IsActive = true });
            data.Users[0].Authentication = new UserAuthentication { Id = data.Users[0].Id, User = data.Users[0], AccessToken = "A" };
            data.Users[1].Authentication = new UserAuthentication { Id = data.Users[1].Id, User = data.Users[1], AccessToken = "B" };
            data.Users[2].Authentication = new UserAuthentication { Id = data.Users[2].Id, User = data.Users[2], AccessToken = "C" };

            return data;
        }
    }
}
