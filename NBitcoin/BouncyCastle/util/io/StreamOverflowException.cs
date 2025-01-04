using System;
using System.IO;

namespace Marscore.NBitcoin.BouncyCastle.util.io
{
    internal class StreamOverflowException
        : IOException
    {
        public StreamOverflowException()
            : base()
        {
        }

        public StreamOverflowException(
            string message)
            : base(message)
        {
        }

        public StreamOverflowException(
            string message,
            Exception exception)
            : base(message, exception)
        {
        }
    }
}
