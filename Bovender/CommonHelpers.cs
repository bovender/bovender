/* CommonHelpers.cs
 * part of Bovender framework
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
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;

namespace Bovender
{
    public static class CommonHelpers
    {
        /// <summary>
        /// Computes the MD5 hash for the object, which must be serializable.
        /// </summary>
        /// <exception cref="ArgumentException">if the object is not
        /// serializable.</exception>
        public static string ComputeMD5Hash(Object obj)
        {
            if (!obj.GetType().IsSerializable)
            {
                throw new ArgumentException("Object must be serializable");
            }

            using (MemoryStream memoryStream = new MemoryStream())
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                lock (_lockObject)
                {
                    binaryFormatter.Serialize(memoryStream, obj);
                }
                MD5CryptoServiceProvider checkSummer = new MD5CryptoServiceProvider();
                memoryStream.Seek(0, SeekOrigin.Begin);
                return ByteArrayToHex(checkSummer.ComputeHash(memoryStream));
            }
        }


        /// <summary>
        /// Converts an array of bytes to the corresponding hexadecimal string.
        /// </summary>
        /// <remarks>
        /// Adapted from http://stackoverflow.com/a/24343727/270712
        /// </remarks>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private static string ByteArrayToHex(byte[] bytes)
        {
            var lookup32 = _lookup32;
            var result = new char[bytes.Length * 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                var val = lookup32[bytes[i]];
                result[2 * i] = (char)val;
                result[2 * i + 1] = (char)(val >> 16);
            }
            return new string(result);
        }

        
        #region Private methods
        
        /// <summary>
        /// Adapted from http://stackoverflow.com/a/24343727/270712
        /// </summary>
        private static uint[] CreateLookup32()
        {
            var result = new uint[256];
            for (int i = 0; i < 256; i++)
            {
                string s = i.ToString("X2");
                result[i] = ((uint)s[0]) + ((uint)s[1] << 16);
            }
            return result;
        }

        #endregion

        #region Private fields

        private static readonly uint[] _lookup32 = CreateLookup32();
        private static readonly Object _lockObject = new Object();

        #endregion
    }
}
