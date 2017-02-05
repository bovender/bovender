/* DllRecord.cs
 * part of Bovender framework
 * 
 * Copyright 2014-2017 Daniel Kraus
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
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Bovender.Unmanaged
{
    /// <summary>
    /// Represents a single DLL file.
    /// </summary>
    class DllFile : IDisposable
    {
        #region Properties

        public string DllPath { get; private set; }

        public IntPtr Handle { get; private set; }

        public int UseCount { get; private set; }

        #endregion

        #region Public methods

        public bool Load()
        {
            Logger.Info("Load: Use count of '{0}' was {1}", DllPath, UseCount);
            bool result = false;
            if (UseCount == 0)
            {
                Logger.Info("Load: Loading DLL...");

                if (!String.IsNullOrWhiteSpace(_expectedSha256))
                {
                    if (!VerifyChecksum())
                    {
                        Logger.Fatal("LoadDll: Checksum mismatch!");
                        throw new DllSha1MismatchException(String.Format(
                            "DLL checksum error: expected {0} on {1}", _expectedSha256, DllPath));
                    }
                };

                Handle = LoadLibrary(DllPath);

                if (Handle == IntPtr.Zero)
                {
                    Logger.Fatal("Load: Unable to load DLL!");
                    Win32Exception inner = new Win32Exception(Marshal.GetLastWin32Error());
                    Logger.Fatal(inner);
                    throw new DllLoadingFailedException(
                        String.Format(
                            "Could not load DLL file: LoadLibrary failed with code {0} on {1}",
                            Marshal.GetLastWin32Error(), SanitizeDllPath()
                        ),
                        inner
                    );
                }
                else
                {
                    Logger.Info("Load: Handle: 0x{0}", Handle.ToString("X8"));
                }
            }

            UseCount++;
            return result;
        }

        public void Unload()
        {
            Logger.Info("Unload: Use count for '{0}' is {1}", DllPath, UseCount);
            if (UseCount <= 0)
            {
                Logger.Warn("Unload: Method call not matched by Load() call!");
            }
            else
            {
                UseCount--;
            }
            if (UseCount == 0)
            {
                Logger.Info("Unload: No more users, freeing handle 0x{0}", Handle.ToString("X8"));
                if (!FreeLibrary(Handle))
                {
                    Logger.Warn("Unload: FreeLibrary returned false");
                }
            }
        }

        #endregion

        #region Constructors

        public DllFile(string path)
        {
            Logger.Info("Constructor: path: {0}", path);
            DllPath = path;
            UseCount = 0;
            Handle = IntPtr.Zero;
        }

        public DllFile(string path, string sha256hash)
            : this(path)
        {
            _expectedSha256 = sha256hash;
        }

        #endregion

        #region Disposing

        ~DllFile()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;
                if (UseCount > 0)
                {
                    FreeLibrary(Handle);
                }
                UseCount = 0;
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Sanitizes a DLL path by removing potentially sensitive information, e.g. the user name.
        /// </summary>
        /// <param name="dllPath">Path to sanitize</param>
        /// <returns></returns>
        private string SanitizeDllPath()
        {
            // Strip the leading directories from the path info (they may contain
            // sensitive information about where exactly a user has installed files).
            string[] dirs = System.IO.Path.GetDirectoryName(DllPath).Split(System.IO.Path.DirectorySeparatorChar);
            string result = DllPath;
            int n = dirs.Length;
            if (n > 0) result = System.IO.Path.Combine(dirs[n - 1], result);
            if (n > 1) result = System.IO.Path.Combine(dirs[n - 2], result);
            if (n > 2) result = System.IO.Path.Combine("...", result);
            Logger.Info("SanitizeDllPath: {0}", result);
            return result;
        }

        private bool VerifyChecksum()
        {
            string actual = FileHelpers.Sha256Hash(DllPath);
            if (actual != _expectedSha256)
            {
                Logger.Warn("VerifyChecksum: Checksum failed for '{0}'", DllPath);
                Logger.Warn("VerifyChecksum: Expected: {0}", _expectedSha256);
                Logger.Warn("VerifyChecksum: Actual:   {0}", actual);
                return false;
            }
            else
            {
                Logger.Info("VerifyChecksum: Confirmed: {0}", _expectedSha256);
                return true;
            }
        }

        #endregion

        #region Private fields

        private bool _disposed;
        private string _expectedSha256;

        #endregion

        #region WinAPI

        [DllImport("kernel32.dll", EntryPoint = "LoadLibrary", SetLastError = true)]
        static extern IntPtr LoadLibrary(string dllToLoad);

        [DllImport("kernel32.dll", EntryPoint = "GetProcAddress", SetLastError = true)]
        static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

        [DllImport("kernel32.dll", EntryPoint = "FreeLibrary", SetLastError = true)]
        static extern bool FreeLibrary(IntPtr hModule);

        #endregion

        #region Class logger

        private static NLog.Logger Logger { get { return _logger.Value; } }

        private static readonly Lazy<NLog.Logger> _logger = new Lazy<NLog.Logger>(() => NLog.LogManager.GetCurrentClassLogger());

        #endregion
    }
}
