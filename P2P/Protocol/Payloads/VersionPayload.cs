﻿using System;
using System.Net;
using System.Reflection;
using Martiscoin.NBitcoin;
using Martiscoin.NBitcoin.DataEncoders;
using Martiscoin.NBitcoin.Protocol;

namespace Martiscoin.P2P.Protocol.Payloads
{
    [Flags]
    public enum NetworkPeerServices : ulong
    {
        Nothing = 0,

        /// <summary>
        /// NODE_NETWORK means that the node is capable of serving the block chain. It is currently
        /// set by all Bitcoin Core nodes, and is unset by SPV clients or other peers that just want
        /// network services but don't provide them.
        /// </summary>
        Network = (1 << 0),

        /// <summary>
        ///  NODE_GETUTXO means the node is capable of responding to the getutxo protocol request.
        /// Bitcoin Core does not support this but a patch set called Bitcoin XT does.
        /// See BIP 64 for details on how this is implemented.
        /// </summary>
        GetUTXO = (1 << 1),

        /// <summary> NODE_BLOOM means the node is capable and willing to handle bloom-filtered connections.
        /// Bitcoin Core nodes used to support this by default, without advertising this bit,
        /// but no longer do as of protocol version 70011 (= NO_BLOOM_VERSION)
        /// </summary>
        NODE_BLOOM = (1 << 2),

        /// <summary> Indicates that a node can be asked for blocks and transactions including
        /// witness data.
        /// </summary>
        NODE_WITNESS = (1 << 3),
    }

    [Payload("version")]
    public class VersionPayload : Payload, IBitcoinSerializable
    {
        private const int MaxSubversionLength = 256;

        private static string userAgentNBitcoin;

        private uint version;

        public uint Version
        {
            get
            {
                // A version number of 10300 is converted to 300 before being processed.
                if (this.version == 10300)
                    return 300;  // https://en.bitcoin.it/wiki/Version_Handshake

                return this.version;
            }

            set
            {
                if (value == 10300)
                    value = 300;

                this.version = (uint)value;
            }
        }

        private ulong services;

        public NetworkPeerServices Services
        {
            get
            {
                return (NetworkPeerServices)this.services;
            }

            set
            {
                this.services = (ulong)value;
            }
        }

        private long timestamp;

        public DateTimeOffset Timestamp
        {
            get
            {
                return Utils.UnixTimeToDateTime((uint)this.timestamp);
            }

            set
            {
                this.timestamp = Utils.DateTimeToUnixTime(value);
            }
        }

        private NetworkAddress addressReceiver = new NetworkAddress();

        public IPEndPoint AddressReceiver
        {
            get
            {
                return this.addressReceiver.Endpoint;
            }

            set
            {
                this.addressReceiver.Endpoint = value ?? throw new InvalidOperationException("Can't set 'AddressReceiver' to null.");
            }
        }

        private NetworkAddress addressFrom = new NetworkAddress();

        public IPEndPoint AddressFrom
        {
            get
            {
                return this.addressFrom.Endpoint;
            }

            set
            {
                this.addressFrom.Endpoint = value ?? throw new InvalidOperationException("Can't set 'AddressFrom' to null.");
            }
        }

        private ulong nonce;

        public ulong Nonce
        {
            get
            {
                return this.nonce;
            }

            set
            {
                this.nonce = value;
            }
        }

        private int startHeight;

        public int StartHeight
        {
            get
            {
                return this.startHeight;
            }

            set
            {
                this.startHeight = value;
            }
        }

        private bool relay = true;

        public bool Relay
        {
            get
            {
                return this.relay;
            }

            set
            {
                this.relay = value;
            }
        }

        private VarString userAgent;

        public string UserAgent
        {
            get
            {
                return Encoders.ASCII.EncodeData(this.userAgent.GetString());
            }

            set
            {
                if (value.Length > MaxSubversionLength)
                    value = value.Substring(0, MaxSubversionLength);

                this.userAgent = new VarString(Encoders.ASCII.DecodeData(value));
            }
        }

        public static string GetNBitcoinUserAgent()
        {
            if (userAgentNBitcoin == null)
            {
                Version version = typeof(VersionPayload).GetTypeInfo().Assembly.GetName().Version;
                userAgentNBitcoin = "/NBitcoin:" + version.Major + "." + version.MajorRevision + "." + version.Build + "/";
            }

            return userAgentNBitcoin;
        }

        public override void ReadWriteCore(BitcoinStream stream)
        {
            stream.ReadWrite(ref this.version);
            using (stream.ProtocolVersionScope(this.version))
            {
                stream.ReadWrite(ref this.services);
                stream.ReadWrite(ref this.timestamp);

                // No time field in version message.
                using (stream.ProtocolVersionScope(ProtocolVersion.CADDR_TIME_VERSION - 1))
                {
                    stream.ReadWrite(ref this.addressReceiver);
                }

                if (this.version >= 106)
                {
                    // No time field in version message.
                    using (stream.ProtocolVersionScope(ProtocolVersion.CADDR_TIME_VERSION - 1))
                    {
                        stream.ReadWrite(ref this.addressFrom);
                    }

                    stream.ReadWrite(ref this.nonce);
                    stream.ReadWrite(ref this.userAgent);
                    if (this.version < 60002)
                    {
                        if (this.userAgent.Length != 0)
                            throw new FormatException("Should not find user agent for current version " + this.version);
                    }

                    stream.ReadWrite(ref this.startHeight);
                    if (this.version >= 70001)
                        stream.ReadWrite(ref this.relay);
                }
            }
        }

        public override string ToString()
        {
            return this.Version.ToString();
        }
    }
}