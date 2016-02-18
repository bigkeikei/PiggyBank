using System;

namespace SimpleIdentity.Models
{
    public class SimpleIdentityUserException : Exception
    {
        public SimpleIdentityUserException(string message) : base(message) { }
    }

    public class SimpleIdentityDataException : Exception
    {
        public SimpleIdentityDataException(string message) : base(message) { }
    }

    public class SimpleIdentityDataConcurrencyException : Exception
    {
        public SimpleIdentityDataConcurrencyException(string message) : base(message) { }
    }

    public class SimpleIdentityDataNotFoundException : Exception
    {
        public SimpleIdentityDataNotFoundException(string message) : base(message) { }
    }
    public class SimpleIdentityAuthenticationTimeoutException : Exception
    {
        public SimpleIdentityAuthenticationTimeoutException(string message) : base(message) { }
    }
    public class SimpleIdentityNotImplementedException : Exception
    {
        public SimpleIdentityNotImplementedException(string message) : base(message) { }
    }


}
