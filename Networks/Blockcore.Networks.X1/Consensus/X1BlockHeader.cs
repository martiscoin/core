using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;
using Blockcore.Consensus.BlockInfo;
using Blockcore.NBitcoin;
using Blockcore.NBitcoin.BouncyCastle.math;
using Blockcore.NBitcoin.Crypto;
using Blockcore.Utilities.JsonErrors;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Blockcore.Networks.X1.Consensus
{
    public class X1BlockHeader : PosBlockHeader
    {
        public uint256 lotPowLimit = new uint256("0000ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff");

        public override uint256 GetPoWHash()
        {
            byte[] serialized;

            using (var ms = new MemoryStream())
            {
                this.ReadWriteHashingStream(new BitcoinStream(ms, true));
                serialized = ms.ToArray();
            }

            return Sha512T.GetHash(serialized);
        }

        public bool CheckLotProofOfWork(byte[] header, uint nonce)
        {
            var bytes = BitConverter.GetBytes(nonce);
            header[76] = bytes[0];
            header[77] = bytes[1];
            header[78] = bytes[2];
            header[79] = bytes[3];
            uint256 headerHash = Sha512T.GetHash(header);
            var res = headerHash <= this.lotPowLimit;
            return res;
        }
    }
}