using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PH.Data
{
    public class Encoder
    {
        public static string Encode(string source)
        {
            if (string.IsNullOrEmpty(source))
                return null;
            return EncodeSimple("abcd"+source);
        }

        public static string Decode(string source)
        {
            if (string.IsNullOrEmpty(source))
                return null;
            if (source.Length <=4)
                return "undefined";

            return EncodeSimple(source.Substring(4));
        }

        private static string EncodeSimple(string source)
        {
            if (string.IsNullOrEmpty(source))
                return null;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < source.Length; i++)
                sb.Append((char) (((char)(source[i])) ^ 1984));
            return sb.ToString();
        }
    }
}
