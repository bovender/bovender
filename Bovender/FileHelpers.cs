/* FileHelpers.cs
 * part of Daniel's XL Toolbox NG
 * 
 * Copyright 2014-2016 Daniel Kraus
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace Bovender
{
    public static class FileHelpers
    {
        #region Public methods

        /// <summary>
        /// Computes the Sha256 hash of a given file.
        /// </summary>
        /// <param name="file">File to compute the Sha256 for.</param>
        /// <returns>Sha1 hash.</returns>
        public static string Sha256Hash(string file)
        {
            using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (BufferedStream bs = new BufferedStream(fs))
            {
                using (SHA256Managed sha = new SHA256Managed())
                {
                    return Checksum2Hash(sha.ComputeHash(bs));
                }
            }
        }

        /// <summary>
        /// Computes the Sha1 hash of a given file.
        /// </summary>
        /// <param name="file">File to compute the Sha1 for.</param>
        /// <returns>Sha1 hash.</returns>
        public static string Sha1Hash(string file)
        {
            using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (BufferedStream bs = new BufferedStream(fs))
            {
                using (SHA1Managed sha = new SHA1Managed())
                {
                    return Checksum2Hash(sha.ComputeHash(bs));
                }
            }
        }

        #endregion

        #region Private helpers
        
        private static string Checksum2Hash(byte[] bytes)
        {
            StringBuilder formatted = new StringBuilder(2 * bytes.Length);
            foreach (byte b in bytes)
            {
                formatted.AppendFormat("{0:x2}", b);
            }
            return formatted.ToString();
        }
        
        #endregion
    }
}
