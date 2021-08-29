using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace ChatClient.Utils
{
    static class Utilities
    {
        public static string Margin_EditOneVector(string toEdit, double valueToAdd, string vector)
        {
            double[] splitted = Array.ConvertAll(toEdit.Split(","), s => double.Parse(s));
            IDictionary<string, int> vectorDispatcher = new Dictionary<string, int>();
            vectorDispatcher.Add("top", 0);
            vectorDispatcher.Add("right", 1);
            vectorDispatcher.Add("bottom", 2);
            vectorDispatcher.Add("left", 3);
            splitted[vectorDispatcher[vector]] += valueToAdd;
            return string.Join(",", splitted);
        }

        public static string ConstructToken(string mail, string pwd_sha1)
        {
            string output = $"::({mail}):({pwd_sha1}):::-";
            output = HashSHA256(HashMD5(output));
            return output;
        }

        public static string HashSHA1(string input)
        {
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
                var sb = new StringBuilder(hash.Length * 2);

                foreach (byte b in hash)
                {
                    sb.Append(b.ToString("X2"));
                }

                return sb.ToString();
            }
        }

        public static string HashSHA256(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public static string HashMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
    }
}
