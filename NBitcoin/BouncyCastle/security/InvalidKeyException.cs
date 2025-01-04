using System;

namespace Marscore.NBitcoin.BouncyCastle.security
{
    internal class InvalidKeyException : KeyException
    {
        public InvalidKeyException() : base() { }
        public InvalidKeyException(string message) : base(message) { }
        public InvalidKeyException(string message, Exception exception) : base(message, exception) { }
    }
}
