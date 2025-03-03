﻿using Martiscoin.NBitcoin;

namespace Martiscoin.P2P.Protocol.Payloads
{
    public class UnknowPayload : Payload
    {
        private string command;

        public override string Command
        {
            get { return this.command; }
        }

        private byte[] data = new byte[0];

        public byte[] Data { get { return this.data; } set { this.data = value; } }

        public UnknowPayload()
        {
        }

        public UnknowPayload(string command)
        {
            this.command = command;
        }

        public override void ReadWriteCore(BitcoinStream stream)
        {
            stream.ReadWrite(ref this.data);
        }

        internal void UpdateCommand(string command)
        {
            this.command = command;
        }
    }
}