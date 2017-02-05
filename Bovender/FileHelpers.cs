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
        /// <returns>Sha256 hash.</returns>
        public static string Sha256Hash(string file)
        {
            using (FileStream fs = SafeFileStream(file))
            {
                using (BufferedStream bs = new BufferedStream(fs))
                {
                    using (SHA256Managed sha = new SHA256Managed())
                    {
                        string hash = Checksum2Hash(sha.ComputeHash(bs));
                        Logger.Info("Sha256Hash: {0}", file);
                        Logger.Info("Sha256Hash: {0}", hash);
                        return hash;
                    }
                }
            }
        }
        /// <summary>
        /// Computes the Sha256 hash of an exception.
        /// </summary>
        /// <param name="exception">Exception to compute the Sha256 for.</param>
        /// <returns>Sha256 hash.</returns>
        public static string Sha256Hash(Exception exception)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(exception.ToString());
            using (SHA256Managed sha = new SHA256Managed())
            {
                string hash = Checksum2Hash(sha.ComputeHash(bytes));
                Logger.Info("Sha256Hash: {0}", exception.GetType().ToString());
                Logger.Info("Sha256Hash: {0}", hash);
                return hash;
            }
        }

        /// <summary>
        /// Computes the Sha1 hash of a given file.
        /// </summary>
        /// <param name="file">File to compute the Sha1 for.</param>
        /// <returns>Sha1 hash.</returns>
        public static string Sha1Hash(string file)
        {
            using (FileStream fs = SafeFileStream(file))
            {
                using (BufferedStream bs = new BufferedStream(fs))
                {
                    using (SHA1Managed sha = new SHA1Managed())
                    {
                        string hash = Checksum2Hash(sha.ComputeHash(bs));
                        Logger.Info("Sha1Hash: {0}", file);
                        Logger.Info("Sha1Hash: {0}", hash);
                        return hash;
                    }
                }
            }
        }

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

        #region Private methods

        /// <summary>
        /// Makes up to 3 attempts to open a file stream in read mode.
        /// </summary>
        /// <param name="file">File to open</param>
        /// <returns>File stream</returns>
        private static FileStream SafeFileStream(string file)
        {
            int tries = 1;
            FileStream fs = null;
            Exception ex = null;
            Logger.Info("SaveFileStream: Attempting to open \"{0}\"", file);
            while (tries <= 3 && fs == null)
            {
                try
                {
                    fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read);
                }
                catch (Exception e)
                {
                    Logger.Warn("SafeFileStream: Failed to open stream on attempt #{0}", tries);
                    Logger.Warn(e);
                    ex = e;
                    System.Threading.Thread.Sleep(333);
                }
                tries++;
            }
            if (fs == null)
            {
                Logger.Fatal("SafeFileStream: Unable to open stream in #{0} attempts!");
                throw new IOException("Unable to access file", ex);
            }
            return fs;
        }
        
        #endregion

        #region Class logger

        private static NLog.Logger Logger { get { return _logger.Value; } }

        private static readonly Lazy<NLog.Logger> _logger = new Lazy<NLog.Logger>(() => NLog.LogManager.GetCurrentClassLogger());

        #endregion
    }
}
