﻿using System;

namespace Martiscoin.NBitcoin.Protocol
{
    public class ProtocolException : Exception
    {
        public ProtocolException(string message)
            : base(message)
        {
        }
    }
}