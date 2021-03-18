using System;
using System.Security.Cryptography;
using System.Text;

namespace DotNetLibraryAdmin
{
    public class Tools
    {
        /// <summary>
        /// Create a hash of the input text.
        /// </summary>
        /// <param name="input">Input text to create hash from.</param>
        /// <param name="algorithm">Which hash algorithm to use. Defaults to SHA256.</param>
        /// <returns>Hash.</returns>
        public static string CreateHash(string input, HashAlgorithm algorithm = null)
        {
            if (input == null)
            {
                throw new ArgumentNullException(
                    nameof(input),
                    "'input' param cannot be null.");
            }

            // Default to SHA256.
            algorithm ??= SHA256.Create();

            // Convert the input string to a byte array and compute the hash.
            var bytes = algorithm.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            var output = new StringBuilder();

            // Loop through each byte of the hashed data
            // and format each one as a hexadecimal string.
            for (var i = 0; i < bytes.Length; i++)
            {
                output.Append(i.ToString("x2"));
            }

            // Return the hexadecimal string.
            return output.ToString();
        }

        /// <summary>
        /// Verify if input matches hash.
        /// </summary>
        /// <param name="input">Input to hash and compare.</param>
        /// <param name="hash">Hash to compare with.</param>
        /// <param name="algorithm">Which hash algorithm to use. Defaults to SHA256.</param>
        /// <returns>Success.</returns>
        public static bool VerifyHash(string input, string hash, HashAlgorithm algorithm = null)
        {
            if (input == null)
            {
                throw new ArgumentNullException(
                    nameof(input),
                    "'input' param cannot be null.");
            }

            if (hash == null)
            {
                throw new ArgumentNullException(
                    nameof(hash),
                    "'hash' param cannot be null.");
            }

            // Default to SHA256.
            algorithm ??= SHA256.Create();

            // Hash the input.
            var hashOfInput = CreateHash(input, algorithm);

            // Compare the hashes.
            return StringComparer.OrdinalIgnoreCase
                .Compare(hashOfInput, hash) == 0;
        }
    }
}