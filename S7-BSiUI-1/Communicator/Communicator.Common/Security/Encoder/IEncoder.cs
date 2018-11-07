namespace Communicator.Common.Security.Encoder
{
    public interface IEncoder
    {
        /// <summary>
        ///     Encode from UTF-8 (and encrypt beforehand if applicable) to Base64.
        /// </summary>
        /// <param name="message">UTF-8 Message to encode.</param>
        /// <returns>Encoded Base64 message.</returns>
        string Encode(string message);
        /// <summary>
        ///     Decode from Base64 (and encrypt beforehand if applicable) to UTF-8.
        /// </summary>
        /// <param name="message">Base64 message to decode.</param>
        /// <returns>Decoded UTF-8 message.</returns>
        string Decode(string message);
    }
}
