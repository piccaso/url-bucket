using System;
using System.Security.Cryptography;
using System.Text;

namespace UrlBucket.Lib.Helper {
    public static class HashHelper {
        public static string ToObjectName(this Uri url) {
            var hash = Sha256(url.ToString());
            return $"{hash.Substring(0, 3)}/{hash}";
        }

        public static string Sha256(string str) {
            var stringBytes = Encoding.UTF8.GetBytes(str);
            var sb = new StringBuilder();

            using (var hashAlgo = new SHA256Managed()) {
                var hashBytes = hashAlgo.ComputeHash(stringBytes);
                foreach (var b in hashBytes) {
                    sb.Append(b.ToString("x2"));
                }
                
            }
            return sb.ToString();
        }
    }
}
