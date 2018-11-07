using System;
using System.Text;

namespace Communicator.Common.Security.Encoder
{
    /// <summary>
    ///     Provides encoding and decoding between UTF-8 and Base64.
    /// </summary>
    public class NoneEncoder : IEncoder
    {
        public string Encode(string message)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            return Convert.ToBase64String(bytes);
        }

        public string Decode(string message)
        {
            byte[] bytes = Convert.FromBase64String(message);
            return Encoding.UTF8.GetString(bytes);
        }
    }
}