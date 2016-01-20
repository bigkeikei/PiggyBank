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
            User userToCreate = _dbContext.Users.Add(user);
            userToCreate.Authentication = new UserAuthentication();
            _dbContext.SaveChanges();
            return userToCreate;
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
            if (userToUpdate == null) return null;
            if (userToUpdate.Name != user.Name) return null;

            userToUpdate.IsActive = user.IsActive;
            userToUpdate.Email = user.Email;
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
            _dbContext.Books.Add(book);
            _dbContext.SaveChanges();
            return book;
        }

        public Book FindBook(int userid, int bookId)
        {
            return _dbContext.Books.Find(bookId);
        }

        public Book UpdateBook(int userId, Book book)
        {
            if (book == null) return null;

            Book bookToUpdate = FindBook(userId, book.Id);
            if (bookToUpdate == null) return null;

            bookToUpdate.Name = book.Name;
            bookToUpdate.IsValid = book.IsValid;
            bookToUpdate.Currency = book.Currency;
            _dbContext.SaveChanges();
            return bookToUpdate;
        }

        #endregion
    }
}
