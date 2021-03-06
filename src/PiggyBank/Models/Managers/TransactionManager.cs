﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PiggyBank.Models
{
    public class TransactionManager : ITransactionManager
    {
        private IPiggyBankDbContext _dbContext;

        public TransactionManager(IPiggyBankDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Transaction>> ListTransactions(int bookId, DateTime? periodStart, DateTime? periodEnd, int? noOfRecords)
        {
            var options = GenerateOptions(periodStart, periodEnd);
            options.Add(b => b.Book.Id == bookId);
            return await ListTransactions(options, noOfRecords);
        }

        public async Task<long> CountTransactions(int bookId, DateTime? periodStart, DateTime? periodEnd)
        {
            var options = GenerateOptions(periodStart, periodEnd);
            options.Add(b => b.Book.Id == bookId);
            return await CountTransactions(options);
        }

        private List<Expression<Func<Transaction, bool>>> GenerateOptions(DateTime? periodStart, DateTime? periodEnd)
        {
            List<Expression<Func<Transaction, bool>>> options = new List<Expression<Func<Transaction, bool>>>();
            if (periodStart != null) { options.Add(b => b.TransactionDate >= periodStart); }
            if (periodEnd != null) { options.Add(b => b.TransactionDate <= periodEnd); }
            return options;
        }

        public async Task<IEnumerable<Transaction>> ListTransactions(IEnumerable<Expression<Func<Transaction, bool>>> options, int? noOfRecords)
        {
            const int recordLimit = 100;
            IQueryable<Transaction> q = _dbContext.Transactions;
            foreach(Expression<Func<Transaction, bool>> exp in options)
            {
                q = q.Where(exp);
            }
            return await q.Where(b => b.IsValid)
                .Include(b => b.CreditAccount)
                .Include(b => b.DebitAccount)
                .Include(b => b.Tags)
                .OrderByDescending(b => b.TransactionDate)
                .Take(noOfRecords??recordLimit)
                .ToListAsync();
        }

        public async Task<long> CountTransactions(IEnumerable<Expression<Func<Transaction, bool>>> options)
        {
            IQueryable<Transaction> q = _dbContext.Transactions;
            foreach (Expression<Func<Transaction, bool>> exp in options)
            {
                q = q.Where(exp);
            }
            return await q.Where(b => b.IsValid)
                .LongCountAsync();
        }

        public async Task<Transaction> CreateTransaction(Book book, Transaction transaction)
        {
            DateTime timeStamp = DateTime.Now;

            if (book == null) throw new PiggyBankDataException("Book object is missing");
            if (transaction == null) throw new PiggyBankDataException("Transaction object is missing");

            transaction.Book = book;
            AccountManager acc = new AccountManager(_dbContext);
            transaction.DebitAccount = await acc.FindAccount(transaction.DebitAccount.Id);
            transaction.CreditAccount = await acc.FindAccount(transaction.CreditAccount.Id);
            ValidateTransaction(transaction);
            transaction.TimeStamp = timeStamp;
            PiggyBankUtility.CheckMandatory(transaction);
            _dbContext.Transactions.Add(transaction);
            await _dbContext.SaveChangesAsync();

            return transaction;
        }

        public async Task<Transaction> FindTransaction(int transactionId, bool populateBook = false)
        {
            var q = _dbContext.Transactions.Where(b => b.Id == transactionId)
                .Include(b => b.CreditAccount)
                .Include(b => b.DebitAccount)
                .Include(b => b.Tags);
            if (populateBook) { q = q.Include(b => b.Book); }
            var transactions = await q.ToListAsync();
            if (!transactions.Any()) throw new PiggyBankDataNotFoundException("Transaction [" + transactionId + "] cannot be found");
            return transactions.First();
        }

        public async Task<Transaction> UpdateTransaction(Transaction transaction)
        {
            DateTime timeStamp = DateTime.Now;
            if (transaction == null) throw new PiggyBankDataException("Transaction object is missing");

            Transaction transactionToUpdate = await FindTransaction(transaction.Id);
            if (!transactionToUpdate.IsValid) throw new PiggyBankDataNotFoundException("Transaction [" + transaction.Id + "] cannot be found");
            if (transactionToUpdate.IsClosed) throw new PiggyBankDataException("Closed Transaction cannot be created / updated");
            if (transaction.TimeStamp != transactionToUpdate.TimeStamp) throw new PiggyBankDataConcurrencyException("Transaction [" + transaction.Id + "] is being updated by other process");

            transaction.Book = transactionToUpdate.Book;
            AccountManager acc = new AccountManager(_dbContext);
            transaction.DebitAccount = await acc.FindAccount(transaction.DebitAccount.Id);
            transaction.CreditAccount = await acc.FindAccount(transaction.CreditAccount.Id);
            ValidateTransaction(transaction);
            transaction.TimeStamp = timeStamp;
            PiggyBankUtility.CheckMandatory(transaction);
            PiggyBankUtility.UpdateModel(transactionToUpdate, transaction);
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) { throw new PiggyBankDataConcurrencyException("Transaction [" + transaction.Id + "] is being updated by other process"); }

            return transactionToUpdate;
        }

        private void ValidateTransaction(Transaction transaction)
        {
            int bookId = transaction.Book.Id;
            string bookCurrency = transaction.Book.Currency;

            // DR/CR account validation
            if (transaction.DebitAccount == null || !transaction.DebitAccount.IsValid) throw new PiggyBankDataException("Invalid Debit Account[" + transaction.DebitAccount.Id + "]");
            if (transaction.CreditAccount == null || !transaction.CreditAccount.IsValid) throw new PiggyBankDataException("Invalid Credit Account[" + transaction.CreditAccount.Id + "]");

            // Book validation
            if (transaction.DebitAccount.Book.Id != bookId) throw new PiggyBankDataException("Debit Account[" + transaction.DebitAccount.Id + "] cannot be found in Book [" + bookId + "]");
            if (transaction.CreditAccount.Book.Id != bookId) throw new PiggyBankDataException("Credit Account[" + transaction.CreditAccount.Id + "] cannot be found in Book [" + bookId + "]");

            // Currency validation
            if (transaction.DebitAccount.Currency != transaction.Currency && transaction.DebitAccount.Currency != bookCurrency) throw new PiggyBankDataException("Invalid DebitAccount.Currency[" + transaction.DebitAccount.Currency + "]");
            if (transaction.CreditAccount.Currency != transaction.Currency && transaction.CreditAccount.Currency != bookCurrency) throw new PiggyBankDataException("Invalid CreditAccount.Currency[" + transaction.CreditAccount.Currency + "]");

            // Book amount validation
            if (transaction.BookAmount <= 0) throw new PiggyBankDataException("Invalid Transaction.BookAmount [" + transaction.BookAmount + "]");

            // Amount validation
            if (transaction.BookAmount < 0) throw new PiggyBankDataException("Invalid Transaction.Amount [" + transaction.Amount + "]");

            // IsClosed validation
            if (transaction.IsClosed) throw new PiggyBankDataException("Closed Transaction cannot be created / updated");

        }

        public async Task AddTag(int transactionId, Tag tag)
        {
            Transaction transaction = await FindTransaction(transactionId);
            if (tag == null) throw new PiggyBankDataException("Tag object is missing");
            TagManager tagManager = new TagManager(_dbContext);
            Tag tagToAdd = await tagManager.FindTag(tag.Id);
            transaction.Tags.Add(tagToAdd);
            await _dbContext.SaveChangesAsync();
        }

        public async Task RemoveTag(int transactionId, int tagId)
        {
            Transaction transaction = await FindTransaction(transactionId);

            var q = transaction.Tags.Where(b => b.Id == tagId);
            if (!q.Any()) throw new PiggyBankDataNotFoundException("Tag [" + tagId + "] cannot be found in Transaction [" + transactionId + "]");
            transaction.Tags.Remove(q.First());
            await _dbContext.SaveChangesAsync();
        }
    }
}
