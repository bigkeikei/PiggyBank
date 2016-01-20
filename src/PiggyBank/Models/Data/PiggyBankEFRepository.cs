using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace PiggyBank.Models.Data
{
    public class PiggyBankEFRepository : IPiggyBankRepository
    {
        PiggyBankDbContext _dbContext;

        #region User Maintenance

        public PiggyBankEFRepository(PiggyBankDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public User CreateUser(User user)
        {
            if (user == null) return null;
            User userCreated = _dbContext.Users.Add(user);
            userCreated.Authentication = new UserAuthentication();
            _dbContext.SaveChanges();
            return userCreated;
        }

        public User FindUser(int userId)
        {
            return _dbContext.Users.Find(userId);
        }

        public User FindUserByName(string userName)
        {
            return _dbContext.Users.Where(b => b.Name == userName).First();
        }

        public User FindUserByToken(string accessToken)
        {
            return _dbContext.Users.Where(b => b.Authentication.AccessToken == accessToken).First();
        }

        public User UpdateUser(User user)
        {
            User userToUpdate = FindUser(user.Id);

            if (userToUpdate == null) throw new PiggyBankDataException("User [" + user.Id + "] cannot be found");
            if (userToUpdate.Name != user.Name) throw new PiggyBankDataException("Editing User.Name is not supported");

            UpdateModel(userToUpdate, user);
            _dbContext.SaveChanges();
            return userToUpdate;
        }

        public IEnumerable<User> ListUsers()
        {
            return _dbContext.Users;
        }

        public UserAuthentication GenerateChallenge(int userId)
        {
            User userToUpdate = FindUser(userId);
            if (userToUpdate == null) return null;

            userToUpdate.Authentication.Challenge = userToUpdate.Name + System.DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
            userToUpdate.Authentication.AccessToken = null;
            userToUpdate.Authentication.RefreshToken = null;
            userToUpdate.Authentication.TokenTimeout = null;
            _dbContext.SaveChanges();
            return userToUpdate.Authentication;
        }

        public UserAuthentication GenerateToken(int userId)
        {
            User userToUpdate = FindUser(userId);
            if (userToUpdate == null) return null;

            userToUpdate.Authentication.AccessToken = Hash(System.Guid.NewGuid().ToString() + userToUpdate.Name);
            userToUpdate.Authentication.RefreshToken = Hash(System.Guid.NewGuid().ToString() + userToUpdate.Name);
            userToUpdate.Authentication.TokenTimeout = System.DateTime.Now.AddMinutes(30);
            _dbContext.SaveChanges();
            return userToUpdate.Authentication;
        }

        private string Hash(string content)
        {
            MD5 md5 = MD5.Create();
            return Convert.ToBase64String(md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(content)));
        }
        #endregion

        #region Book Maintenance

        public Book CreateBook(User user, Book book)
        {
            if (user == null || book == null) return null;
            book.User = user;
            Book bookCreated = _dbContext.Books.Add(book);
            _dbContext.SaveChanges();
            return bookCreated;
        }

        public Book FindBook(int bookId)
        {
            return _dbContext.Books.Find(bookId);
        }

        public Book UpdateBook(Book book)
        {
            if (book == null) return null;

            Book bookToUpdate = FindBook(book.Id);
            if (bookToUpdate == null) throw new PiggyBankDataException("Book [" + book.Id + "] cannot be found");
            UpdateModel(bookToUpdate, book);
            _dbContext.SaveChanges();
            return bookToUpdate;
        }

        #endregion

        #region Account Maintenance

        public Account CreateAccount(Book book, Account account)
        {
            if (book == null || account == null) return null;
            account.Book = book;
            Account accountCreated = _dbContext.Accounts.Add(account);
            _dbContext.SaveChanges();
            return accountCreated;
        }

        public Account FindAccount(int accountId)
        {
            return _dbContext.Accounts.Find(accountId);
        }

        public Account UpdateAccount(Account account)
        {
            if (account == null) return null;
            Account accountToUpdate = FindAccount(account.Id);
            if (accountToUpdate == null) throw new PiggyBankDataException("Account [" + account.Id + "] cannot be found");
            UpdateModel(accountToUpdate, account);
            _dbContext.SaveChanges();
            return accountToUpdate;
        }

        #endregion

        public void UpdateModel<T>(T modelToUpdate, T model)
        {
            var props = typeof(T).GetProperties();
            foreach (var prop in props)
            {
                if (!prop.IsDefined(typeof(PiggyBankEFIgnore), true))
                {
                    prop.SetValue(modelToUpdate, prop.GetValue(model));
                }
            }
        }
    }

    public class PiggyBankDataException : Exception
    {
        public PiggyBankDataException(string message) : base(message) { }
    }
}
