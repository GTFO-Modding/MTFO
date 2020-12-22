using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataDumper.Utilities
{
    public static class DataDumperExtensions
    {
        /// <summary>
        /// Hashes the given string in a persistent, platform independent way
        /// </summary>
        /// Thanks to https://stackoverflow.com/a/36845864 for this
        /// <param name="str">The string to hash</param>
        /// <returns>The hash of the string</returns>
        public static int GetStableHashCode(this string str)
        {
            unchecked
            {
                int hash1 = 5381;
                int hash2 = hash1;

                for (int i = 0; i < str.Length && str[i] != '\0'; i += 2)
                {
                    hash1 = ((hash1 << 5) + hash1) ^ str[i];
                    if (i == str.Length - 1 || str[i + 1] == '\0')
                        break;
                    hash2 = ((hash2 << 5) + hash2) ^ str[i + 1];
                }

                return hash1 + (hash2 * 1566083941);
            }
        }
    }
}
