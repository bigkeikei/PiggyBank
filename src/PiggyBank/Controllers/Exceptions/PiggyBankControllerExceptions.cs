using System;

namespace PiggyBank.Controllers.Exceptions
{
    public class PiggyBankUserException : Exception
    {
        public PiggyBankUserException(string message) : base(message) { }
    }

    public class PiggyBankBookException : Exception
    {
        public PiggyBankBookException(string message) : base(message) { }
    }

    public class PiggyBankAccountException : Exception
    {
        public PiggyBankAccountException(string message) : base(message) { }
    }
}
