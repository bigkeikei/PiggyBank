﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;

namespace PiggyBank.Models.Data
{
    public class PiggyBankDbContext : DbContext
    {
        public PiggyBankDbContext(string connectionString) : base(connectionString)
        {
            if (!Database.Exists())
            {
                Database.Create();
            }
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Account> Accounts { get; set; }
    }
}