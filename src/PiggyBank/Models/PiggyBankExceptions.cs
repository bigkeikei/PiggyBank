using System;

namespace PiggyBank.Models
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

    public class PiggyBankDataException : Exception
    {
        public PiggyBankDataException(string message) : base(message) { }
    }

    public class PiggyBankDataConcurrencyException : Exception
    {
        public PiggyBankDataConcurrencyException(string message) : base(message) { }
    }

    public class PiggyBankDataNotFoundException : Exception
    {
        public PiggyBankDataNotFoundException(string message) : base(message) { }
    }
    public class PiggyBankAuthenticationTimeoutException : Exception
    {
        public PiggyBankAuthenticationTimeoutException(string message) : base(message) { }
    }
    public class PiggyBankNotImplementedException : Exception
    {
        public PiggyBankNotImplementedException(string message) : base(message) { }
    }


}
