﻿using System.Reflection;
using Martiscoin.NBitcoin;

namespace Martiscoin.P2P.Protocol.Payloads
{
    public class Payload : IBitcoinSerializable
    {
        public virtual string Command
        {
            get
            {
                return this.GetType().GetCustomAttribute<PayloadAttribute>().Name;
            }
        }

        public void ReadWrite(BitcoinStream stream)
        {
            using (stream.SerializationTypeScope(SerializationType.Network))
            {
                this.ReadWriteCore(stream);
            }
        }

        public virtual void ReadWriteCore(BitcoinStream stream)
        {
        }

        public override string ToString()
        {
            return this.GetType().Name;
        }
    }
}