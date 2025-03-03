﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Martiscoin.Consensus.Chain;
using Martiscoin.Consensus.ScriptInfo;
using Martiscoin.NBitcoin;
using Martiscoin.NBitcoin.Crypto;
using Martiscoin.NBitcoin.DataEncoders;
using Martiscoin.NBitcoin.Formatters;
using Martiscoin.NBitcoin.Protocol;
using Martiscoin.Networks;

namespace Martiscoin.Consensus.TransactionInfo
{
    public class OutPoint : IBitcoinSerializable
    {
        public bool IsNull
        {
            get
            {
                return (this.hash == uint256.Zero && this.n == uint.MaxValue);
            }
        }

        private uint256 hash = uint256.Zero;
        private uint n;

        public uint256 Hash
        {
            get
            {
                return this.hash;
            }
            set
            {
                this.hash = value;
            }
        }

        public uint N
        {
            get
            {
                return this.n;
            }
            set
            {
                this.n = value;
            }
        }

        public static bool TryParse(string str, out OutPoint result)
        {
            result = null;
            if (str == null)
                throw new ArgumentNullException("str");
            string[] splitted = str.Split('-');
            if (splitted.Length != 2)
                return false;

            uint256 hash;
            if (!uint256.TryParse(splitted[0], out hash))
                return false;

            uint index;
            if (!uint.TryParse(splitted[1], out index))
                return false;
            result = new OutPoint(hash, index);
            return true;
        }

        public static OutPoint Parse(string str)
        {
            OutPoint result;
            if (TryParse(str, out result))
                return result;
            throw new FormatException("The format of the outpoint is incorrect");
        }

        public OutPoint()
        {
            SetNull();
        }

        public OutPoint(uint256 hashIn, uint nIn)
        {
            this.hash = hashIn;
            this.n = nIn;
        }

        public OutPoint(uint256 hashIn, int nIn)
        {
            this.hash = hashIn;
            this.n = nIn == -1 ? this.n = uint.MaxValue : (uint)nIn;
        }

        public OutPoint(Transaction tx, uint i)
            : this(tx.GetHash(), i)
        {
        }

        public OutPoint(Transaction tx, int i)
            : this(tx.GetHash(), i)
        {
        }

        public OutPoint(OutPoint outpoint)
        {
            this.FromBytes(outpoint.ToBytes());
        }

        //IMPLEMENT_SERIALIZE( READWRITE(FLATDATA(*this)); )

        public void ReadWrite(BitcoinStream stream)
        {
            stream.ReadWrite(ref this.hash);
            stream.ReadWrite(ref this.n);
        }

        private void SetNull()
        {
            this.hash = uint256.Zero;
            this.n = uint.MaxValue;
        }

        public static bool operator <(OutPoint a, OutPoint b)
        {
            return (a.hash < b.hash || (a.hash == b.hash && a.n < b.n));
        }

        public static bool operator >(OutPoint a, OutPoint b)
        {
            return (a.hash > b.hash || (a.hash == b.hash && a.n > b.n));
        }

        public static bool operator ==(OutPoint a, OutPoint b)
        {
            if (ReferenceEquals(a, null))
            {
                return ReferenceEquals(b, null);
            }
            if (ReferenceEquals(b, null))
            {
                return false;
            }
            return (a.hash == b.hash && a.n == b.n);
        }

        public static bool operator !=(OutPoint a, OutPoint b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            var item = obj as OutPoint;
            if (ReferenceEquals(null, item))
                return false;
            return item == this;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return 17 + this.hash.GetHashCode() * 31 + this.n.GetHashCode() * 31 * 31;
            }
        }

        public override string ToString()
        {
            return this.Hash + "-" + this.N;
        }
    }

    public class TxIn : IBitcoinSerializable
    {
        public TxIn()
        {
        }

        public TxIn(Script scriptSig)
        {
            this.scriptSig = scriptSig;
        }

        public TxIn(OutPoint prevout, Script scriptSig)
        {
            this.prevout = prevout;
            this.scriptSig = scriptSig;
        }

        public TxIn(OutPoint prevout)
        {
            this.prevout = prevout;
        }

        private OutPoint prevout = new OutPoint();
        private Script scriptSig = Script.Empty;
        private uint nSequence = uint.MaxValue;

        public Sequence Sequence
        {
            get
            {
                return this.nSequence;
            }
            set
            {
                this.nSequence = value.Value;
            }
        }

        public OutPoint PrevOut
        {
            get
            {
                return this.prevout;
            }
            set
            {
                this.prevout = value;
            }
        }

        public Script ScriptSig
        {
            get
            {
                return this.scriptSig;
            }
            set
            {
                this.scriptSig = value;
            }
        }

        /// <summary>
        /// Try to get the expected scriptPubKey of this TxIn based on its scriptSig and witScript.
        /// </summary>
        /// <returns>Null if could not infer the scriptPubKey, else, the expected scriptPubKey</returns>
        public IDestination GetSigner(Network network)
        {
            return this.scriptSig.GetSigner(network) ?? this.witScript.GetSigner(network);
        }

        private WitScript witScript = WitScript.Empty;

        /// <summary>
        /// The witness script (Witness script is not serialized and deserialized at the TxIn level, but at the Transaction level)
        /// </summary>
        public WitScript WitScript
        {
            get
            {
                return this.witScript;
            }
            set
            {
                this.witScript = value;
            }
        }

        public bool WitScriptEmpty
        {
            get
            {
                return this.WitScript == WitScript.Empty || this.WitScript == null;
            }
        }

        #region IBitcoinSerializable Members

        public void ReadWrite(BitcoinStream stream)
        {
            stream.ReadWrite(ref this.prevout);
            stream.ReadWrite(ref this.scriptSig);
            stream.ReadWrite(ref this.nSequence);
        }

        #endregion IBitcoinSerializable Members

        public bool IsFrom(Network network, PubKey pubKey)
        {
            PayToPubkeyHashScriptSigParameters result = PayToPubkeyHashTemplate.Instance.ExtractScriptSigParameters(network, this.ScriptSig);
            return result != null && result.PublicKey == pubKey;
        }

        public bool IsFinal
        {
            get
            {
                return (this.nSequence == uint.MaxValue);
            }
        }

        public static TxIn CreateCoinbase(int height)
        {
            var txin = new TxIn();
            txin.ScriptSig = new Script(Op.GetPushOp(height)) + OpcodeType.OP_0;
            return txin;
        }

        public override string ToString()
        {
            return this.PrevOut.ToString();
        }
    }

    public class TxOutCompressor : IBitcoinSerializable
    {
        // Amount compression:
        // * If the amount is 0, output 0
        // * first, divide the amount (in base units) by the largest power of 10 possible; call the exponent e (e is max 9)
        // * if e<9, the last digit of the resulting number cannot be 0; store it as d, and drop it (divide by 10)
        //   * call the result n
        //   * output 1 + 10*(9*n + d - 1) + e
        // * if e==9, we only know the resulting number is not zero, so output 1 + 10*(n - 1) + 9
        // (this is decodable, as d is in [1-9] and e is in [0-9])

        private ulong CompressAmount(ulong n)
        {
            if (n == 0)
                return 0;
            int e = 0;
            while (((n % 10) == 0) && e < 9)
            {
                n /= 10;
                e++;
            }
            if (e < 9)
            {
                int d = (int)(n % 10);
                n /= 10;
                return 1 + (n * 9 + (ulong)(d - 1)) * 10 + (ulong)e;
            }
            else
            {
                return 1 + (n - 1) * 10 + 9;
            }
        }

        private ulong DecompressAmount(ulong x)
        {
            // x = 0  OR  x = 1+10*(9*n + d - 1) + e  OR  x = 1+10*(n - 1) + 9
            if (x == 0)
                return 0;
            x--;
            // x = 10*(9*n + d - 1) + e
            int e = (int)(x % 10);
            x /= 10;
            ulong n = 0;
            if (e < 9)
            {
                // x = 9*n + d - 1
                int d = (int)((x % 9) + 1);
                x /= 9;
                // x = n
                n = (x * 10 + (ulong)d);
            }
            else
            {
                n = x + 1;
            }
            while (e != 0)
            {
                n *= 10;
                e--;
            }
            return n;
        }

        private TxOut _TxOut = new TxOut();

        public TxOut TxOut
        {
            get
            {
                return this._TxOut;
            }
        }

        public TxOutCompressor()
        {
        }

        public TxOutCompressor(TxOut txOut)
        {
            this._TxOut = txOut;
        }

        #region IBitcoinSerializable Members

        public void ReadWrite(BitcoinStream stream)
        {
            if (stream.Serializing)
            {
                ulong val = CompressAmount((ulong)this._TxOut.Value.Satoshi);
                stream.ReadWriteAsCompactVarInt(ref val);
            }
            else
            {
                ulong val = 0;
                stream.ReadWriteAsCompactVarInt(ref val);
                this._TxOut.Value = new Money(DecompressAmount(val));
            }
            var cscript = new ScriptCompressor(this._TxOut.ScriptPubKey);
            stream.ReadWrite(ref cscript);
            if (!stream.Serializing) this._TxOut.ScriptPubKey = new Script(cscript.ScriptBytes);
        }

        #endregion IBitcoinSerializable Members
    }

    public class ScriptCompressor : IBitcoinSerializable
    {
        // make this static for now (there are only 6 special scripts defined)
        // this can potentially be extended together with a new nVersion for
        // transactions, in which case this value becomes dependent on nVersion
        // and nHeight of the enclosing transaction.
        private const uint nSpecialScripts = 6;

        private byte[] _Script;

        public byte[] ScriptBytes
        {
            get
            {
                return this._Script;
            }
        }

        public ScriptCompressor(Script script)
        {
            this._Script = script.ToBytes(true);
        }

        public ScriptCompressor()
        {
        }

        public Script GetScript()
        {
            return new Script(this._Script);
        }

        private byte[] Compress()
        {
            byte[] result = null;
            Script script = Script.FromBytesUnsafe(this._Script);
            KeyId keyID = PayToPubkeyHashTemplate.Instance.ExtractScriptPubKeyParameters(script);
            if (keyID != null)
            {
                result = new byte[21];
                result[0] = 0x00;
                Array.Copy(keyID.ToBytes(true), 0, result, 1, 20);
                return result;
            }
            ScriptId scriptID = PayToScriptHashTemplate.Instance.ExtractScriptPubKeyParameters(script);
            if (scriptID != null)
            {
                result = new byte[21];
                result[0] = 0x01;
                Array.Copy(scriptID.ToBytes(true), 0, result, 1, 20);
                return result;
            }
            PubKey pubkey = PayToPubkeyTemplate.Instance.ExtractScriptPubKeyParameters(script, true);
            if (pubkey != null)
            {
                result = new byte[33];
                byte[] pubBytes = pubkey.ToBytes(true);
                Array.Copy(pubBytes, 1, result, 1, 32);
                if (pubBytes[0] == 0x02 || pubBytes[0] == 0x03)
                {
                    result[0] = pubBytes[0];
                    return result;
                }
                else if (pubBytes[0] == 0x04)
                {
                    result[0] = (byte)(0x04 | (pubBytes[64] & 0x01));
                    return result;
                }
            }
            return null;
        }

        private Script Decompress(uint nSize, byte[] data)
        {
            switch (nSize)
            {
                case 0x00:
                    return PayToPubkeyHashTemplate.Instance.GenerateScriptPubKey(new KeyId(data.SafeSubarray(0, 20)));

                case 0x01:
                    return PayToScriptHashTemplate.Instance.GenerateScriptPubKey(new ScriptId(data.SafeSubarray(0, 20)));

                case 0x02:
                case 0x03:
                    byte[] keyPart = data.SafeSubarray(0, 32);
                    var keyBytes = new byte[33];
                    keyBytes[0] = (byte)nSize;
                    Array.Copy(keyPart, 0, keyBytes, 1, 32);
                    return PayToPubkeyTemplate.Instance.GenerateScriptPubKey(keyBytes);

                case 0x04:
                case 0x05:
                    var vch = new byte[33];
                    vch[0] = (byte)(nSize - 2);
                    Array.Copy(data, 0, vch, 1, 32);
                    var pubkey = new PubKey(vch, true);
                    pubkey = pubkey.Decompress();
                    return PayToPubkeyTemplate.Instance.GenerateScriptPubKey(pubkey);
            }
            return null;
        }

        #region IBitcoinSerializable Members

        public void ReadWrite(BitcoinStream stream)
        {
            if (stream.Serializing)
            {
                byte[] compr = Compress();
                if (compr != null)
                {
                    stream.ReadWrite(ref compr);
                    return;
                }
                uint nSize = (uint)this._Script.Length + nSpecialScripts;
                stream.ReadWriteAsCompactVarInt(ref nSize);
                stream.ReadWrite(ref this._Script);
            }
            else
            {
                uint nSize = 0;
                stream.ReadWriteAsCompactVarInt(ref nSize);
                if (nSize < nSpecialScripts)
                {
                    var vch = new byte[GetSpecialSize(nSize)];
                    stream.ReadWrite(ref vch);
                    this._Script = Decompress(nSize, vch).ToBytes();
                    return;
                }
                nSize -= nSpecialScripts;
                this._Script = new byte[nSize];
                stream.ReadWrite(ref this._Script);
            }
        }

        private int GetSpecialSize(uint nSize)
        {
            if (nSize == 0 || nSize == 1)
                return 20;
            if (nSize == 2 || nSize == 3 || nSize == 4 || nSize == 5)
                return 32;
            return 0;
        }

        #endregion IBitcoinSerializable Members
    }

    public class TxOut : IBitcoinSerializable, IDestination
    {
        private Script publicKey = Script.Empty;

        public Script ScriptPubKey
        {
            get
            {
                return this.publicKey;
            }
            set
            {
                this.publicKey = value;
            }
        }

        public bool IsEmpty => (this.Value == 0 && this.ScriptPubKey.Length == 0);

        public TxOut()
        {
        }

        public TxOut(Money value, IDestination destination)
        {
            this.Value = value;
            if (destination != null) this.ScriptPubKey = destination.ScriptPubKey;
        }

        public TxOut(Money value, Script scriptPubKey)
        {
            this.Value = value;
            this.ScriptPubKey = scriptPubKey;
        }

        private readonly static Money NullMoney = new Money(-1);
        private Money _Value = NullMoney;

        public Money Value
        {
            get
            {
                return this._Value;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                this._Value = value;
            }
        }

        public bool IsDust(FeeRate minRelayTxFee)
        {
            return (this.Value < GetDustThreshold(minRelayTxFee));
        }

        public Money GetDustThreshold(FeeRate minRelayTxFee)
        {
            if (minRelayTxFee == null)
                throw new ArgumentNullException("minRelayTxFee");
            int nSize = this.GetSerializedSize() + 148;
            return 3 * minRelayTxFee.GetFee(nSize);
        }

        #region IBitcoinSerializable Members

        public void ReadWrite(BitcoinStream stream)
        {
            long value = this.Value.Satoshi;
            stream.ReadWrite(ref value);
            if (!stream.Serializing) this._Value = new Money(value);
            stream.ReadWrite(ref this.publicKey);
        }

        #endregion IBitcoinSerializable Members

        public bool IsTo(IDestination destination)
        {
            return this.ScriptPubKey == destination.ScriptPubKey;
        }

        public static TxOut Parse(string hex)
        {
            var ret = new TxOut();
            ret.FromBytes(Encoders.Hex.DecodeData(hex));
            return ret;
        }

        public override string ToString()
        {
            return $"{nameof(this.Value)}={this.Value},{nameof(this.ScriptPubKey)}={this.ScriptPubKey}";
        }
    }

    public class IndexedTxIn
    {
        public TxIn TxIn
        {
            get;
            set;
        }

        /// <summary>
        /// The index of this TxIn in its transaction
        /// </summary>
        public uint Index
        {
            get;
            set;
        }

        public OutPoint PrevOut
        {
            get
            {
                return this.TxIn.PrevOut;
            }
            set
            {
                this.TxIn.PrevOut = value;
            }
        }

        public Script ScriptSig
        {
            get
            {
                return this.TxIn.ScriptSig;
            }
            set
            {
                this.TxIn.ScriptSig = value;
            }
        }

        public WitScript WitScript
        {
            get
            {
                return this.TxIn.WitScript;
            }
            set
            {
                this.TxIn.WitScript = value;
            }
        }

        public Transaction Transaction
        {
            get;
            set;
        }

        public bool VerifyScript(Network network, Script scriptPubKey, ScriptVerify scriptVerify = ScriptVerify.Standard)
        {
            return VerifyScript(network, scriptPubKey, scriptVerify, out ScriptError unused);
        }

        public bool VerifyScript(Network network, Script scriptPubKey, out ScriptError error)
        {
            return Script.VerifyScript(network, scriptPubKey, this.Transaction, (int)this.Index, null, out error);
        }

        public bool VerifyScript(Network network, Script scriptPubKey, ScriptVerify scriptVerify, out ScriptError error)
        {
            return Script.VerifyScript(network, scriptPubKey, this.Transaction, (int)this.Index, null, scriptVerify, SigHash.Undefined, out error);
        }

        public bool VerifyScript(Network network, Script scriptPubKey, Money value, ScriptVerify scriptVerify, out ScriptError error)
        {
            return Script.VerifyScript(network, scriptPubKey, this.Transaction, (int)this.Index, value, scriptVerify, SigHash.Undefined, out error);
        }

        public bool VerifyScript(Network network, ICoin coin, ScriptVerify scriptVerify = ScriptVerify.Standard)
        {
            return VerifyScript(network, coin, scriptVerify, out ScriptError unused);
        }

        public bool VerifyScript(Network network, ICoin coin, ScriptVerify scriptVerify, out ScriptError error)
        {
            return Script.VerifyScript(network, coin.TxOut.ScriptPubKey, this.Transaction, (int)this.Index, coin.TxOut.Value, scriptVerify, SigHash.Undefined, out error);
        }

        public bool VerifyScript(Network network, ICoin coin, out ScriptError error)
        {
            return VerifyScript(network, coin, ScriptVerify.Standard, out error);
        }

        public TransactionSignature Sign(Network network, Key key, ICoin coin, SigHash sigHash)
        {
            uint256 hash = GetSignatureHash(network, coin, sigHash);
            return key.Sign(hash, sigHash);
        }

        public uint256 GetSignatureHash(Network network, ICoin coin, SigHash sigHash = SigHash.All)
        {
            return Script.SignatureHash(network, coin.GetScriptCode(network), this.Transaction, (int)this.Index, sigHash, coin.TxOut.Value, coin.GetHashVersion(network));
        }
    }

    public class TxInList : UnsignedList<TxIn>
    {
        public TxInList()
        {
        }

        public TxInList(Transaction parent)
            : base(parent)
        {
        }

        public TxIn this[OutPoint outpoint]
        {
            get
            {
                return this[outpoint.N];
            }
            set
            {
                this[outpoint.N] = value;
            }
        }

        public IEnumerable<IndexedTxIn> AsIndexedInputs()
        {
            // We want i as the index of txIn in Intputs[], not index in enumerable after where filter
            return this.Select((r, i) => new IndexedTxIn()
            {
                TxIn = r,
                Index = (uint)i,
                Transaction = this.Transaction
            });
        }
    }

    public class IndexedTxOut
    {
        public TxOut TxOut
        {
            get;
            set;
        }

        public uint N
        {
            get;
            set;
        }

        public Transaction Transaction
        {
            get;
            set;
        }

        public Coin ToCoin()
        {
            return new Coin(this);
        }

        public OutPoint ToOutPoint()
        {
            return new OutPoint(this.Transaction, this.N);
        }
    }

    public class TxOutList : UnsignedList<TxOut>
    {
        public TxOutList()
        {
        }

        public TxOutList(Transaction parent)
            : base(parent)
        {
        }

        public IEnumerable<TxOut> To(IDestination destination)
        {
            return this.Where(r => r.IsTo(destination));
        }

        public IEnumerable<TxOut> To(Script scriptPubKey)
        {
            return this.Where(r => r.ScriptPubKey == scriptPubKey);
        }

        public IEnumerable<IndexedTxOut> AsIndexedOutputs()
        {
            // We want i as the index of txOut in Outputs[], not index in enumerable after where filter
            return this.Select((r, i) => new IndexedTxOut()
            {
                TxOut = r,
                N = (uint)i,
                Transaction = this.Transaction
            });
        }

        public IEnumerable<Coin> AsCoins()
        {
            uint256 txId = this.Transaction.GetHash();
            for (int i = 0; i < this.Count; i++)
            {
                yield return new Coin(new OutPoint(txId, i), this[i]);
            }
        }
    }

    public enum RawFormat
    {
        Satoshi,
        BlockExplorer,
    }

    public class WitScript
    {
        private byte[][] _Pushes;

        public WitScript(string script)
        {
            string[] parts = script.Split(new[] { '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            this._Pushes = new byte[parts.Length][];
            for (int i = 0; i < parts.Length; i++)
            {
                this._Pushes[i] = Encoders.Hex.DecodeData(parts[i]);
            }
        }

        /// <summary>
        /// Create a new WitnessScript
        /// </summary>
        /// <param name="script">Scripts</param>
        /// <param name="unsafe">If false, make a copy of the input script array</param>
        public WitScript(byte[][] script, bool @unsafe = false)
        {
            if (@unsafe)
                this._Pushes = script;
            else
            {
                this._Pushes = script.ToArray();
                for (int i = 0; i < this._Pushes.Length; i++) this._Pushes[i] = script[i].ToArray();
            }
        }

        /// <summary>
        /// Create a new WitnessScript
        /// </summary>
        /// <param name="script">Scripts</param>
        public WitScript(IEnumerable<byte[]> script, bool @unsafe = false)
            : this(script.ToArray(), @unsafe)
        {
        }

        public WitScript(params Op[] ops)
        {
            var pushes = new List<byte[]>();
            foreach (Op op in ops)
            {
                if (op.PushData == null)
                    throw new ArgumentException("Non push operation unsupported in WitScript", "ops");
                pushes.Add(op.PushData);
            }

            this._Pushes = pushes.ToArray();
        }

        public WitScript(byte[] script)
        {
            if (script == null)
                throw new ArgumentNullException("script");

            using (var ms = new MemoryStream(script))
            {
                var stream = new BitcoinStream(ms, false);
                ReadCore(stream);
            }
        }

        private WitScript()
        {
        }

        public WitScript(Script scriptSig)
        {
            var pushes = new List<byte[]>();
            foreach (Op op in scriptSig.ToOps())
            {
                if (op.PushData == null)
                    throw new ArgumentException("A WitScript can only contains push operations", "script");
                pushes.Add(op.PushData);
            }

            this._Pushes = pushes.ToArray();
        }

        public static WitScript Load(BitcoinStream stream)
        {
            var script = new WitScript();
            script.ReadCore(stream);
            return script;
        }

        private void ReadCore(BitcoinStream stream)
        {
            var pushes = new List<byte[]>();
            uint pushCount = 0;
            stream.ReadWriteAsVarInt(ref pushCount);
            for (int i = 0; i < (int)pushCount; i++)
            {
                byte[] push = ReadPush(stream);
                pushes.Add(push);
            }

            this._Pushes = pushes.ToArray();
        }

        private static byte[] ReadPush(BitcoinStream stream)
        {
            byte[] push = null;
            stream.ReadWriteAsVarString(ref push);
            return push;
        }

        public byte[] this[int index]
        {
            get
            {
                return this._Pushes[index];
            }
        }

        public IEnumerable<byte[]> Pushes
        {
            get
            {
                return this._Pushes;
            }
        }

        private static WitScript _Empty = new WitScript(new byte[0][], true);

        public static WitScript Empty
        {
            get
            {
                return _Empty;
            }
        }

        public override bool Equals(object obj)
        {
            var item = obj as WitScript;
            if (item == null)
                return false;
            return EqualsCore(item);
        }

        private bool EqualsCore(WitScript item)
        {
            if (this._Pushes.Length != item._Pushes.Length)
                return false;
            for (int i = 0; i < this._Pushes.Length; i++)
            {
                if (!Utils.ArrayEqual(this._Pushes[i], item._Pushes[i]))
                    return false;
            }
            return true;
        }

        public static bool operator ==(WitScript a, WitScript b)
        {
            if (ReferenceEquals(a, b))
                return true;
            if (((object)a == null) || ((object)b == null))
                return false;
            return a.EqualsCore(b);
        }

        public static bool operator !=(WitScript a, WitScript b)
        {
            return !(a == b);
        }

        public static WitScript operator +(WitScript a, WitScript b)
        {
            if (a == null)
                return b;
            if (b == null)
                return a;
            return new WitScript(a._Pushes.Concat(b._Pushes).ToArray());
        }

        public static implicit operator Script(WitScript witScript)
        {
            if (witScript == null)
                return null;
            return witScript.ToScript();
        }

        public override int GetHashCode()
        {
            return Utils.GetHashCode(ToBytes());
        }

        public byte[] ToBytes()
        {
            using (var ms = new MemoryStream())
            {
                var stream = new BitcoinStream(ms, true);
                uint pushCount = (uint)this._Pushes.Length;
                stream.ReadWriteAsVarInt(ref pushCount);
                foreach (byte[] push in this.Pushes)
                {
                    byte[] localpush = push;
                    stream.ReadWriteAsVarString(ref localpush);
                }
                return ms.ToArrayEfficient();
            }
        }

        public override string ToString()
        {
            return ToScript().ToString();
        }

        public Script ToScript()
        {
            return new Script(this._Pushes.Select(p => Op.GetPushOp(p)).ToArray());
        }

        public int PushCount
        {
            get
            {
                return this._Pushes.Length;
            }
        }

        public byte[] GetUnsafePush(int i)
        {
            return this._Pushes[i];
        }

        public WitScript Clone()
        {
            return new WitScript(ToBytes());
        }

        public TxDestination GetSigner(Network network)
        {
            PayToWitPubkeyHashScriptSigParameters pubKey = PayToWitPubKeyHashTemplate.Instance.ExtractWitScriptParameters(network, this);
            if (pubKey != null)
            {
                return pubKey.PublicKey.WitHash;
            }
            Script p2sh = PayToWitScriptHashTemplate.Instance.ExtractWitScriptParameters(this);
            return p2sh != null ? p2sh.WitHash : null;
        }
    }

    [Flags]
    public enum TransactionOptions : uint
    {
        None = 0x00000000,
        Witness = 0x40000000,
        All = Witness
    }

    internal class Witness
    {
        private TxInList _Inputs;

        public Witness(TxInList inputs)
        {
            this._Inputs = inputs;
        }

        internal bool IsNull()
        {
            return this._Inputs.All(i => i.WitScript.PushCount == 0);
        }

        internal void ReadWrite(BitcoinStream stream)
        {
            for (int i = 0; i < this._Inputs.Count; i++)
            {
                if (stream.Serializing)
                {
                    byte[] bytes = (this._Inputs[i].WitScript ?? WitScript.Empty).ToBytes();
                    stream.ReadWrite(ref bytes);
                }
                else
                {
                    this._Inputs[i].WitScript = WitScript.Load(stream);
                }
            }

            if (IsNull())
                throw new FormatException("Superfluous witness record");
        }
    }

    //https://en.bitcoin.it/wiki/Transactions
    //https://en.bitcoin.it/wiki/Protocol_specification
    public class Transaction : IBitcoinSerializable
    {
        public bool RBF
        {
            get
            {
                return this.Inputs.Any(i => i.Sequence < 0xffffffff - 1);
            }
        }

        protected uint nVersion = 1;

        public uint Version
        {
            get
            {
                return this.nVersion;
            }
            set
            {
                this.nVersion = value;
            }
        }

        protected TxInList vin;
        protected TxOutList vout;
        protected LockTime nLockTime;

        public Transaction()
        {
            this.vin = new TxInList(this);
            this.vout = new TxOutList(this);
        }

        public Money TotalOut
        {
            get
            {
                return this.Outputs.Sum(v => v.Value);
            }
        }

        public LockTime LockTime
        {
            get
            {
                return this.nLockTime;
            }
            set
            {
                this.nLockTime = value;
            }
        }

        public TxInList Inputs
        {
            get
            {
                return this.vin;
            }
        }

        public TxOutList Outputs
        {
            get
            {
                return this.vout;
            }
        }

        //Since it is impossible to serialize a transaction with 0 input without problems during deserialization with wit activated, we fit a flag in the version to workaround it
        protected const uint NoDummyInput = (1 << 27);

        #region IBitcoinSerializable Members

        public virtual void ReadWrite(BitcoinStream stream)
        {
            bool witSupported = (((uint)stream.TransactionOptions & (uint)TransactionOptions.Witness) != 0) &&
                                stream.ProtocolVersion >= ProtocolVersion.WITNESS_VERSION;

            byte flags = 0;
            if (!stream.Serializing)
            {
                stream.ReadWrite(ref this.nVersion);

                /* Try to read the vin. In case the dummy is there, this will be read as an empty vector. */
                stream.ReadWrite<TxInList, TxIn>(ref this.vin);

                bool hasNoDummy = (this.nVersion & NoDummyInput) != 0 && this.vin.Count == 0;
                if (witSupported && hasNoDummy) this.nVersion = this.nVersion & ~NoDummyInput;

                if (this.vin.Count == 0 && witSupported && !hasNoDummy)
                {
                    /* We read a dummy or an empty vin. */
                    stream.ReadWrite(ref flags);
                    if (flags != 0)
                    {
                        /* Assume we read a dummy and a flag. */
                        stream.ReadWrite<TxInList, TxIn>(ref this.vin);
                        this.vin.Transaction = this;
                        stream.ReadWrite<TxOutList, TxOut>(ref this.vout);
                        this.vout.Transaction = this;
                    }
                    else
                    {
                        /* Assume read a transaction without output. */
                        this.vout = new TxOutList();
                        this.vout.Transaction = this;
                    }
                }
                else
                {
                    /* We read a non-empty vin. Assume a normal vout follows. */
                    stream.ReadWrite<TxOutList, TxOut>(ref this.vout);
                    this.vout.Transaction = this;
                }
                if (((flags & 1) != 0) && witSupported)
                {
                    /* The witness flag is present, and we support witnesses. */
                    flags ^= 1;
                    var wit = new Witness(this.Inputs);
                    wit.ReadWrite(stream);
                }
                if (flags != 0)
                {
                    /* Unknown flag in the serialization */
                    throw new FormatException("Unknown transaction optional data");
                }
            }
            else
            {
                uint version = (witSupported && (this.vin.Count == 0 && this.vout.Count > 0)) ? this.nVersion | NoDummyInput : this.nVersion;
                stream.ReadWrite(ref version);

                if (witSupported)
                {
                    /* Check whether witnesses need to be serialized. */
                    if (this.HasWitness)
                    {
                        flags |= 1;
                    }
                }
                if (flags != 0)
                {
                    /* Use extended format in case witnesses are to be serialized. */
                    var vinDummy = new TxInList();
                    stream.ReadWrite<TxInList, TxIn>(ref vinDummy);
                    stream.ReadWrite(ref flags);
                }
                stream.ReadWrite<TxInList, TxIn>(ref this.vin);
                this.vin.Transaction = this;
                stream.ReadWrite<TxOutList, TxOut>(ref this.vout);
                this.vout.Transaction = this;
                if ((flags & 1) != 0)
                {
                    var wit = new Witness(this.Inputs);
                    wit.ReadWrite(stream);
                }
            }
            stream.ReadWriteStruct(ref this.nLockTime);
        }

        #endregion IBitcoinSerializable Members

        public uint256 GetHash()
        {
            uint256 h = null;
            uint256[] hashes = this.hashes;
            if (hashes != null)
            {
                h = hashes[0];
            }
            if (h != null)
                return h;

            using (var hs = new HashStream())
            {
                ReadWrite(new BitcoinStream(hs, true)
                {
                    TransactionOptions = TransactionOptions.None
                });
                h = hs.GetHash();
            }

            hashes = this.hashes;
            if (hashes != null)
            {
                hashes[0] = h;
            }
            return h;
        }

        [Obsolete("Call PrecomputeHash(true, true) instead")]
        public void CacheHashes()
        {
            PrecomputeHash(true, true);
        }

        /// <summary>
        /// Precompute the transaction hash and witness hash so that later calls to GetHash() and GetWitHash() will returns the precomputed hash
        /// </summary>
        /// <param name="invalidateExisting">If true, the previous precomputed hash is thrown away, else it is reused</param>
        /// <param name="lazily">If true, the hash will be calculated and cached at the first call to GetHash(), else it will be immediately</param>
        public void PrecomputeHash(bool invalidateExisting, bool lazily)
        {
            this.hashes = invalidateExisting ? new uint256[2] : this.hashes ?? new uint256[2];

            if (!lazily && this.hashes[0] == null)
                this.hashes[0] = this.GetHash();

            if (!lazily && this.hashes[1] == null)
                this.hashes[1] = this.GetWitHash();
        }

        private uint256[] hashes = null;

        public uint256 GetWitHash()
        {
            if (!this.HasWitness)
                return GetHash();

            uint256 h = null;
            uint256[] hashes = this.hashes;
            if (hashes != null)
            {
                h = hashes[1];
            }
            if (h != null)
                return h;

            using (var hs = new HashStream())
            {
                ReadWrite(new BitcoinStream(hs, true)
                {
                    TransactionOptions = TransactionOptions.Witness
                });
                h = hs.GetHash();
            }

            hashes = this.hashes;
            if (hashes != null)
            {
                hashes[1] = h;
            }
            return h;
        }

        public uint256 GetSignatureHash(Network network, ICoin coin, SigHash sigHash = SigHash.All)
        {
            return this.Inputs.AsIndexedInputs().ToArray()[GetIndex(coin)].GetSignatureHash(network, coin, sigHash);
        }

        public TransactionSignature SignInput(Network network, ISecret secret, ICoin coin, SigHash sigHash = SigHash.All)
        {
            return SignInput(network, secret.PrivateKey, coin, sigHash);
        }

        public TransactionSignature SignInput(Network network, Key key, ICoin coin, SigHash sigHash = SigHash.All)
        {
            return this.Inputs.AsIndexedInputs().ToArray()[GetIndex(coin)].Sign(network, key, coin, sigHash);
        }

        private int GetIndex(ICoin coin)
        {
            for (int i = 0; i < this.Inputs.Count; i++)
            {
                if (this.Inputs[i].PrevOut == coin.Outpoint)
                    return i;
            }
            throw new ArgumentException("The coin is not being spent by this transaction", "coin");
        }

        public bool IsCoinBase
        {
            get
            {
                return this.Inputs.Count == 1
                    && this.Inputs.First().PrevOut.IsNull
                    && this.Outputs.Count >= 1;
            }
        }

        public bool IsCoinStake
        {
            get
            {
                // ppcoin: the coin stake transaction is marked with the first output empty
                return this.Inputs.Any()
                    && !this.Inputs.First().PrevOut.IsNull
                    && this.Outputs.Count() >= 2
                    && this.Outputs.First().IsEmpty;
            }
        }

        /// <summary>
        /// This flag is used to carry state between the script engine and the
        /// cold staking rule checks, it should not be used outside of that context.
        /// </summary>
        public bool IsColdCoinStake { get; set; }

        public virtual bool IsProtocolTransaction()
        {
            return this.IsCoinBase;
        }

        public static uint CURRENT_VERSION = 2;
        public static uint MAX_STANDARD_TX_SIZE = 100000;

        public TxOut AddOutput(Money money, IDestination destination)
        {
            return AddOutput(new TxOut(money, destination));
        }

        public TxOut AddOutput(Money money, Script scriptPubKey)
        {
            return AddOutput(new TxOut(money, scriptPubKey));
        }

        public TxOut AddOutput(TxOut @out)
        {
            this.vout.Add(@out);
            return @out;
        }

        public TxIn AddInput(TxIn @in)
        {
            this.vin.Add(@in);
            return @in;
        }

        /// <summary>
        /// Size of the transaction discounting the witness (Used for fee calculation)
        /// </summary>
        /// <param name="witnessScaleFactor">Witness scale factor from network settings (i.e. 4 for BTC).</param>
        /// <returns>Transaction size</returns>
        public int GetVirtualSize(int witnessScaleFactor)
        {
            int totalSize = this.GetSerializedSize(TransactionOptions.Witness);
            int strippedSize = this.GetSerializedSize(TransactionOptions.None);
            // This implements the weight = (stripped_size * 4) + witness_size formula,
            // using only serialization with and without witness data. As witness_size
            // is equal to total_size - stripped_size, this formula is identical to:
            // weight = (stripped_size * 3) + total_size.
            int weight = strippedSize * (witnessScaleFactor - 1) + totalSize;
            return (weight + witnessScaleFactor - 1) / witnessScaleFactor;
        }

        public TxIn AddInput(Transaction prevTx, int outIndex)
        {
            if (outIndex >= prevTx.Outputs.Count)
                throw new InvalidOperationException("Output " + outIndex + " is not present in the prevTx");
            var @in = new TxIn();
            @in.PrevOut.Hash = prevTx.GetHash();
            @in.PrevOut.N = (uint)outIndex;
            AddInput(@in);
            return @in;
        }

        /// <summary>
        /// Sign a specific coin with the given secret
        /// </summary>
        /// <param name="secrets">Secrets</param>
        /// <param name="coins">Coins to sign</param>
        public void Sign(Network network, ISecret[] secrets, ICoin[] coins)
        {
            Sign(network, secrets.Select(s => s.PrivateKey).ToArray(), coins);
        }

        /// <summary>
        /// Sign a specific coin with the given secret
        /// </summary>
        /// <param name="keys">Private keys</param>
        /// <param name="coins">Coins to sign</param>
        public void Sign(Network network, Key[] keys, ICoin[] coins)
        {
            var builder = new TransactionBuilder(network);
            builder.AddKeys(keys);
            builder.AddCoins(coins);
            builder.SignTransactionInPlace(this);
        }

        /// <summary>
        /// Sign a specific coin with the given secret
        /// </summary>
        /// <param name="secret">Secret</param>
        /// <param name="coins">Coins to sign</param>
        public void Sign(Network network, ISecret secret, ICoin[] coins)
        {
            Sign(network, new[] { secret }, coins);
        }

        /// <summary>
        /// Sign a specific coin with the given secret
        /// </summary>
        /// <param name="secrets">Secrets</param>
        /// <param name="coins">Coins to sign</param>
        public void Sign(Network network, ISecret[] secrets, ICoin coin)
        {
            Sign(network, secrets, new[] { coin });
        }

        /// <summary>
        /// Sign a specific coin with the given secret
        /// </summary>
        /// <param name="secret">Secret</param>
        /// <param name="coin">Coins to sign</param>
        public void Sign(Network network, ISecret secret, ICoin coin)
        {
            Sign(network, new[] { secret }, new[] { coin });
        }

        /// <summary>
        /// Sign a specific coin with the given secret
        /// </summary>
        /// <param name="key">Private key</param>
        /// <param name="coins">Coins to sign</param>
        public void Sign(Network network, Key key, ICoin[] coins)
        {
            Sign(network, new[] { key }, coins);
        }

        /// <summary>
        /// Sign a specific coin with the given secret
        /// </summary>
        /// <param name="key">Private key</param>
        /// <param name="coin">Coin to sign</param>
        public void Sign(Network network, Key key, ICoin coin)
        {
            Sign(network, new[] { key }, new[] { coin });
        }

        /// <summary>
        /// Sign a specific coin with the given secret
        /// </summary>
        /// <param name="keys">Private keys</param>
        /// <param name="coin">Coin to sign</param>
        public void Sign(Network network, Key[] keys, ICoin coin)
        {
            Sign(network, keys, new[] { coin });
        }

        /// <summary>
        /// Sign the transaction with a private key
        /// <para>ScriptSigs should be filled with previous ScriptPubKeys</para>
        /// <para>For more complex scenario, use TransactionBuilder</para>
        /// </summary>
        /// <param name="secret"></param>
        [Obsolete("Use Sign(ISecret,ICoin[]) instead)")]
        public void Sign(Network network, ISecret secret, bool assumeP2SH)
        {
            Sign(network, secret.PrivateKey, assumeP2SH);
        }

        /// <summary>
        /// Sign the transaction with a private key
        /// <para>ScriptSigs should be filled with either previous scriptPubKeys or redeem script (for P2SH)</para>
        /// <para>For more complex scenario, use TransactionBuilder</para>
        /// </summary>
        /// <param name="key"></param>
        [Obsolete("Use Sign(Key,ICoin[]) instead)")]
        public void Sign(Network network, Key key, bool assumeP2SH)
        {
            var coins = new List<Coin>();
            for (int i = 0; i < this.Inputs.Count; i++)
            {
                TxIn txin = this.Inputs[i];
                if (Script.IsNullOrEmpty(txin.ScriptSig))
                    throw new InvalidOperationException("ScriptSigs should be filled with either previous scriptPubKeys or redeem script (for P2SH)");
                if (assumeP2SH)
                {
                    PayToScriptHashSigParameters p2shSig = PayToScriptHashTemplate.Instance.ExtractScriptSigParameters(network, txin.ScriptSig);
                    if (p2shSig == null)
                    {
                        coins.Add(new ScriptCoin(txin.PrevOut, new TxOut()
                        {
                            ScriptPubKey = txin.ScriptSig.PaymentScript,
                        }, txin.ScriptSig));
                    }
                    else
                    {
                        coins.Add(new ScriptCoin(txin.PrevOut, new TxOut()
                        {
                            ScriptPubKey = p2shSig.RedeemScript.PaymentScript
                        }, p2shSig.RedeemScript));
                    }
                }
                else
                {
                    coins.Add(new Coin(txin.PrevOut, new TxOut()
                    {
                        ScriptPubKey = txin.ScriptSig
                    }));
                }
            }
            Sign(network, key, coins.ToArray());
        }

        /*
        public TxPayload CreatePayload()
        {
            return new TxPayload(this.Clone());
        }
        */

        public static Transaction Parse(string tx, RawFormat format, Network network = null)
        {
            return GetFormatter(format, network).ParseJson(tx);
        }

        public string ToHex()
        {
            return Encoders.Hex.EncodeData(this.ToBytes());
        }

        public override string ToString()
        {
            return this.GetHash().ToString();
        }

        public string ToString(Network network, RawFormat rawFormat = RawFormat.BlockExplorer)
        {
            RawFormatter formatter = GetFormatter(rawFormat, network);
            return ToString(formatter);
        }

        static private RawFormatter GetFormatter(RawFormat rawFormat, Network network)
        {
            RawFormatter formatter = null;
            switch (rawFormat)
            {
                case RawFormat.Satoshi:
                    formatter = new SatoshiFormatter(network);
                    break;

                case RawFormat.BlockExplorer:
                    formatter = new BlockExplorerFormatter(network);
                    break;

                default:
                    throw new NotSupportedException(rawFormat.ToString());
            }
            return formatter;
        }

        internal string ToString(RawFormatter formatter)
        {
            if (formatter == null)
                throw new ArgumentNullException("formatter");
            return formatter.ToString(this);
        }

        /// <summary>
        /// Calculate the fee of the transaction
        /// </summary>
        /// <param name="spentCoins">Coins being spent</param>
        /// <returns>Fee or null if some spent coins are missing or if spentCoins is null</returns>
        public Money GetFee(ICoin[] spentCoins)
        {
            if (this.IsCoinBase)
                return Money.Zero;
            spentCoins = spentCoins ?? new ICoin[0];

            Money fees = -this.TotalOut;
            foreach (TxIn input in this.Inputs)
            {
                ICoin coin = spentCoins.FirstOrDefault(s => s.Outpoint == input.PrevOut);
                if (coin == null)
                    return null;
                fees += coin.TxOut.Value;
            }
            return fees;
        }

        /// <summary>
        /// Calculate the fee rate of the transaction
        /// </summary>
        /// <param name="witnessScaleFactor">Witness scale factor from network settings (i.e. 4 for BTC).</param>
        /// <param name="spentCoins">Coins being spent</param>
        /// <returns>Fee or null if some spent coins are missing or if spentCoins is null</returns>
        public FeeRate GetFeeRate(int witnessScaleFactor, ICoin[] spentCoins)
        {
            Money fee = GetFee(spentCoins);
            if (fee == null)
                return null;
            return new FeeRate(fee, GetVirtualSize(witnessScaleFactor));
        }

        public bool IsFinal(ChainedHeader block)
        {
            if (block == null)
                return IsFinal(Utils.UnixTimeToDateTime(0), 0);

            return IsFinal(block.Header.BlockTime, block.Height);
        }

        public bool IsFinal(DateTimeOffset blockTime, int blockHeight)
        {
            uint nBlockTime = Utils.DateTimeToUnixTime(blockTime);

            if (this.nLockTime == 0)
                return true;

            if ((long)this.nLockTime < ((long)this.nLockTime < LockTime.LOCKTIME_THRESHOLD ? (long)blockHeight : nBlockTime))
                return true;

            foreach (TxIn txin in this.Inputs)
            {
                if (!txin.IsFinal)
                    return false;
            }

            return true;
        }

        [Flags]
        public enum LockTimeFlags : int
        {
            None = 0,

            /// <summary>
            /// Interpret sequence numbers as relative lock-time constraints.
            /// </summary>
            VerifySequence = (1 << 0),

            /// <summary>
            ///  Use GetMedianTimePast() instead of nTime for end point timestamp.
            /// </summary>
            MedianTimePast = (1 << 1),
        }

        /// <summary>
        /// Calculates the block height and time which the transaction must be later than
        /// in order to be considered final in the context of BIP 68.  It also removes
        /// from the vector of input heights any entries which did not correspond to sequence
        /// locked inputs as they do not affect the calculation.
        /// </summary>
        /// <param name="prevHeights">Previous Height</param>
        /// <param name="block">The block being evaluated</param>
        /// <param name="flags">If VerifySequence is not set, returns always true SequenceLock</param>
        /// <returns>Sequence lock of minimum SequenceLock to satisfy</returns>
        public bool CheckSequenceLocks(int[] prevHeights, ChainedHeader block, LockTimeFlags flags = LockTimeFlags.VerifySequence)
        {
            return CalculateSequenceLocks(prevHeights, block, flags).Evaluate(block);
        }

        /// <summary>
        /// Calculates the block height and time which the transaction must be later than
        /// in order to be considered final in the context of BIP 68.  It also removes
        /// from the vector of input heights any entries which did not correspond to sequence
        /// locked inputs as they do not affect the calculation.
        /// </summary>
        /// <param name="prevHeights">Previous Height</param>
        /// <param name="chainedHeader">The Chained block header being evaluated</param>
        /// <param name="flags">If VerifySequence is not set, returns always true SequenceLock</param>
        /// <returns>Sequence lock of minimum SequenceLock to satisfy</returns>
        public SequenceLock CalculateSequenceLocks(int[] prevHeights, ChainedHeader chainedHeader, LockTimeFlags flags = LockTimeFlags.VerifySequence)
        {
            if (prevHeights.Length != this.Inputs.Count)
                throw new ArgumentException("The number of element in prevHeights should be equal to the number of inputs", "prevHeights");

            // Will be set to the equivalent height- and time-based nLockTime
            // values that would be necessary to satisfy all relative lock-
            // time constraints given our view of block chain history.
            // The semantics of nLockTime are the last invalid height/time, so
            // use -1 to have the effect of any height or time being valid.
            int nMinHeight = -1;
            long nMinTime = -1;

            // tx.nVersion is signed integer so requires cast to unsigned otherwise
            // we would be doing a signed comparison and half the range of nVersion
            // wouldn't support BIP 68.
            bool fEnforceBIP68 = this.Version >= 2
                              && (flags & LockTimeFlags.VerifySequence) != 0;

            // Do not enforce sequence numbers as a relative lock time
            // unless we have been instructed to
            if (!fEnforceBIP68)
            {
                return new SequenceLock(nMinHeight, nMinTime);
            }

            for (int txinIndex = 0; txinIndex < this.Inputs.Count; txinIndex++)
            {
                TxIn txin = this.Inputs[txinIndex];

                // Sequence numbers with the most significant bit set are not
                // treated as relative lock-times, nor are they given any
                // consensus-enforced meaning at this point.
                if ((txin.Sequence & Sequence.SEQUENCE_LOCKTIME_DISABLE_FLAG) != 0)
                {
                    // The height of this input is not relevant for sequence locks
                    prevHeights[txinIndex] = 0;
                    continue;
                }

                int nCoinHeight = prevHeights[txinIndex];

                if ((txin.Sequence & Sequence.SEQUENCE_LOCKTIME_TYPE_FLAG) != 0)
                {
                    long nCoinTime = (long)Utils.DateTimeToUnixTimeLong(chainedHeader.GetAncestor(Math.Max(nCoinHeight - 1, 0)).GetMedianTimePast());

                    // Time-based relative lock-times are measured from the
                    // smallest allowed timestamp of the block containing the
                    // txout being spent, which is the median time past of the
                    // block prior.
                    nMinTime = Math.Max(nMinTime, nCoinTime + (long)((txin.Sequence & Sequence.SEQUENCE_LOCKTIME_MASK) << Sequence.SEQUENCE_LOCKTIME_GRANULARITY) - 1);
                }
                else
                {
                    // We subtract 1 from relative lock-times because a lock-
                    // time of 0 has the semantics of "same block," so a lock-
                    // time of 1 should mean "next block," but nLockTime has
                    // the semantics of "last invalid block height."
                    nMinHeight = Math.Max(nMinHeight, nCoinHeight + (int)(txin.Sequence & Sequence.SEQUENCE_LOCKTIME_MASK) - 1);
                }
            }

            return new SequenceLock(nMinHeight, nMinTime);
        }

        /// <summary>
        /// Create a transaction with the specified option only. (useful for stripping data from a transaction)
        /// </summary>
        /// <param name="options">Options to keep</param>
        /// <param name="consensusFactory">The network consensus factory.</param>
        /// <returns>A new transaction with only the options wanted</returns>
        public Transaction WithOptions(TransactionOptions options, ConsensusFactory consensusFactory)
        {
            if (options == TransactionOptions.Witness && this.HasWitness)
                return this;
            if (options == TransactionOptions.None && !this.HasWitness)
                return this;
            Transaction instance = consensusFactory.CreateTransaction();
            using (var ms = new MemoryStream())
            {
                var bms = new BitcoinStream(ms, true, consensusFactory);
                bms.TransactionOptions = options;
                this.ReadWrite(bms);
                ms.Position = 0;
                bms = new BitcoinStream(ms, false, consensusFactory);
                bms.TransactionOptions = options;
                instance.ReadWrite(bms);
                return instance;
            }
        }

        public bool HasWitness
        {
            get
            {
                return this.Inputs.Any(i => i.WitScriptEmpty == false);
            }
        }

        private static readonly uint MAX_BLOCK_SIZE = 1000000;
        private static readonly ulong MAX_MONEY = long.MaxValue; // 21000000ul * Money.COIN;

        /// <summary>
        /// Context free transaction check
        /// </summary>
        /// <returns>The error or success of the check</returns>
        public TransactionCheckResult Check()
        {
            // Basic checks that don't depend on any context
            if (this.Inputs.Count == 0)
                return TransactionCheckResult.NoInput;
            if (this.Outputs.Count == 0)
                return TransactionCheckResult.NoOutput;
            // Size limits
            if (this.GetSerializedSize() > MAX_BLOCK_SIZE)
                return TransactionCheckResult.TransactionTooLarge;

            // Check for negative or overflow output values
            long nValueOut = 0;
            foreach (TxOut txout in this.Outputs)
            {
                if (txout.Value < 0)
                    return TransactionCheckResult.NegativeOutput;
                if (txout.Value > MAX_MONEY)
                    return TransactionCheckResult.OutputTooLarge;
                nValueOut += txout.Value;
                if (!((nValueOut >= 0 && nValueOut <= (long)MAX_MONEY)))
                    return TransactionCheckResult.OutputTotalTooLarge;
            }

            // Check for duplicate inputs
            var vInOutPoints = new HashSet<OutPoint>();
            foreach (TxIn txin in this.Inputs)
            {
                if (vInOutPoints.Contains(txin.PrevOut))
                    return TransactionCheckResult.DuplicateInputs;
                vInOutPoints.Add(txin.PrevOut);
            }

            if (this.IsCoinBase)
            {
                if (this.Inputs[0].ScriptSig.Length < 2 || this.Inputs[0].ScriptSig.Length > 100)
                    return TransactionCheckResult.CoinbaseScriptTooLarge;
            }
            else
            {
                foreach (TxIn txin in this.Inputs)
                {
                    if (txin.PrevOut.IsNull)
                        return TransactionCheckResult.NullInputPrevOut;
                }
            }

            return TransactionCheckResult.Success;
        }
    }

    public enum TransactionCheckResult
    {
        Success,
        NoInput,
        NoOutput,
        NegativeOutput,
        OutputTooLarge,
        OutputTotalTooLarge,
        TransactionTooLarge,
        DuplicateInputs,
        NullInputPrevOut,
        CoinbaseScriptTooLarge,
    }
}