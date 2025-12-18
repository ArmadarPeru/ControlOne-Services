using System;
using System.Security.Cryptography;
using System.Text;

namespace Armadar.Helpers
{
   public class HashMacHelper
   {
      public static bool CheckHash(string expectedAlgorithm, string algoritmo, string krAnswer, string krHash, string hashSecretKey)
      {
         if (algoritmo == expectedAlgorithm)
         {
            return krHash == HashHmac(krAnswer, hashSecretKey);
         }
         else { return false; }
      }

      private static string HashHmac(string message, string secret)
      {
         Encoding encoding = Encoding.UTF8;
         using (HMACSHA256 hmac = new HMACSHA256(encoding.GetBytes(secret)))
         {
            var msg = encoding.GetBytes(message);
            var hash = hmac.ComputeHash(msg);
            return BitConverter.ToString(hash).ToLower().Replace("-", string.Empty);
         }
      }
   }
}
