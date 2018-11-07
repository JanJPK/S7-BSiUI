using System;
using System.Numerics;
using System.Text;

namespace Communicator.Common.Security.Encoder
{
    /// <summary>
    ///     Provides encoding and decoding between UTF-8 and Base64.
    ///     Encrypts by shifting characters.
    /// </summary>
    public class CaesarEncoder : IEncoder
    {
        private readonly BigInteger sharedSecret;

        public CaesarEncoder(BigInteger sharedSecret)
        {
            this.sharedSecret = sharedSecret;
        }

        public string Encode(string message)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            var encrypted = Convert.ToBase64String(Encrypt(bytes));
            return encrypted;
        }

        public string Decode(string message)
        {
            byte[] bytes = Convert.FromBase64String(message);
            var decrypted = Encoding.UTF8.GetString(Decrypt(bytes));
            return decrypted;
        }

        private byte[] Encrypt(byte[] bytes)
        {
            byte[] output = new byte[bytes.Length];

            for (var i = 0; i < bytes.Length; i++)
            {
                var added = (bytes[i] + sharedSecret) % 256;
                output[i] = (byte) added;
            }

            return output;
        }

        private byte[] Decrypt(byte[] bytes)
        {
            byte[] output = new byte[bytes.Length];

            for (var i = 0; i < bytes.Length; i++)
            {
                var subtracted = bytes[i] - sharedSecret % 256;
                if (subtracted < 0)
                {
                    subtracted += 256;
                }

                output[i] = (byte) subtracted;
            }

            return output;
        }
    }
}