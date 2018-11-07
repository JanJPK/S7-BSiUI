using System;
using System.Linq;
using System.Numerics;
using System.Text;

namespace Communicator.Common.Security.Encoder
{
    /// <summary>
    ///     Provides encoding and decoding between UTF-8 and Base64.
    ///     Encrypts by xor.
    /// </summary>
    public class XorEncoder : IEncoder
    {
        private readonly byte xorMask;

        public XorEncoder(BigInteger sharedSecret)
        {
            var bytes = sharedSecret.ToByteArray();
            xorMask = bytes.First();
        }

        public string Encode(string message)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            var encrypted = Convert.ToBase64String(XorBytes(bytes));
            return encrypted;
        }

        public string Decode(string message)
        {
            byte[] bytes = Convert.FromBase64String(message);
            var decrypted = Encoding.UTF8.GetString(XorBytes(bytes));
            return decrypted;
        }

        private byte[] XorBytes(byte[] bytes)
        {
            byte[] output = new byte[bytes.Length];

            for (var i = 0; i < bytes.Length; i++)
            {
                output[i] = (byte) (bytes[i] ^ xorMask);
            }

            return output;
        }
    }
}