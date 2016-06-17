/* DllManager.cs
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
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace Bovender.Unmanaged
{
    /// <summary>
    /// Manages unmanaged DLLs. Unloads any loaded DLLs upon disposal.
    /// </summary>
    public class DllManager : Object, IDisposable
    {
        #region Constants

        private const string LIBDIR = "lib";

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets an alternative dir where DLL files 
        /// </summary>
        public virtual string AlternativeDir { get; set; }

        #endregion

        #region WinAPI

        [DllImport("kernel32.dll", EntryPoint="LoadLibrary", SetLastError=true)]
        static extern IntPtr LoadLibrary(string dllToLoad);

        [DllImport("kernel32.dll", EntryPoint="GetProcAddress", SetLastError=true)]
        static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

        [DllImport("kernel32.dll", EntryPoint = "FreeLibrary", SetLastError = true)]
        static extern bool FreeLibrary(IntPtr hModule);

        #endregion
        
        #region Private members
        /// <summary>
        /// Holds the currently loaded DLL names and their handles.
        /// </summary>
        private Dictionary<string, IntPtr> _loadedDlls = new Dictionary<string, IntPtr>();

        private bool _disposed;
        #endregion

        #region Loading and unloading DLLs

        /// <summary>
        /// Loads the given DLL from the appropriate subdirectory, depending on the current
        /// bitness.
        /// </summary>
        /// <remarks>
        /// The DLL is expected to reside in a subdirectory of the entry point assembly's
        /// directory, in "bin/lib/$(Platform)", where $(Platform) can be "Win32" or "x64",
        /// for example. If DllManager.AlternativeDir is set, the subdirectories there will be
        /// queried as well.
        /// </remarks>
        /// <exception cref="DllNotFoundException">if the file is not found in the path.</exception>
        /// <exception cref="DllLoadingFailedException">if the file could not be loaded.</exception>
        /// <param name="dllName">Name of the DLL to load (without path).</param>
        public void LoadDll(string dllName)
        {
            LoadDll(dllName, String.Empty);
        }

        /// <summary>
        /// Loads the given DLL from the appropriate subdirectory if its Sha1 hash
        /// matches the provided hash.
        /// </summary>
        /// <remarks>
        /// The DLL is expected to reside in a subdirectory of the entry point assembly's
        /// directory, in "bin/lib/$(Platform)", where $(Platform) can be "Win32" or "x64",
        /// for example. If DllManager.AlternativeDir is set, the subdirectories there will be
        /// queried as well.
        /// </remarks>
        /// <param name="dllName">Name of the DLL to load (without path).</param>
        /// <param name="expectedSha1Hash">Expected Sha1 hash of the DLL.</param>
        /// <exception cref="DllNotFoundException">if the file is not found in the path.</exception>
        /// <exception cref="DllLoadingFailedException">if the file could not be loaded.</exception>
        /// <exception cref="DllSha1MismatchException">if the file's Sha1 is unexpected.</exception>
        // TODO: Use two expected hashes, one for Win32, one for x64
        public void LoadDll(string dllName, string expectedSha1Hash)
        {
            Logger.Info("LoadDll: {0}", dllName);

            string dllPath = LocateDll(dllName);
            if (String.IsNullOrEmpty(dllPath))
            {
                throw new DllNotFoundException(
                    String.Format("Unable to locate DLL file"));
            }
            Logger.Info("Path: {0}", dllName);

            if (!String.IsNullOrWhiteSpace(expectedSha1Hash))
            {
                Logger.Info("Verifying checksum, expected: {0}", expectedSha1Hash);
                if (!VerifyChecksum(dllPath, expectedSha1Hash))
                {
                    Logger.Fatal("DLL checksum mismatch, expected {0} on {1}", expectedSha1Hash, dllPath);
                    throw new DllSha1MismatchException(String.Format(
                        "DLL checksum error: expected {0} on {2}", expectedSha1Hash, dllPath));
                }
            };

            IntPtr handle = LoadLibrary(dllPath);
            if (handle == IntPtr.Zero)
            {
                Win32Exception inner = new Win32Exception(Marshal.GetLastWin32Error());
                Logger.Fatal(inner);
                throw new DllLoadingFailedException(
                    String.Format(
                        "Could not load DLL file: LoadLibrary failed with code {0} on {1}",
                        Marshal.GetLastWin32Error(), SanitizeDllPath(dllPath)
                    ),
                    inner
                );
            }

            // Register the DLL and its handle in the internal database
            _loadedDlls.Add(dllName, handle);
        }

        /// <summary>
        /// Unloads a previously loaded DLL. Does nothing if the DLL was not loaded.
        /// </summary>
        /// <param name="dllName">Name of the DLL to unload.</param>
        public void UnloadDll(string dllName)
        {
            Logger.Info("UnloadDll");
            IntPtr handle;
            if (_loadedDlls.TryGetValue(dllName, out handle))
            {
                FreeLibrary(handle);
            }
        }

        #endregion

        #region Destructing and disposing

        ~DllManager()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool calledFromDispose)
        {
            if (_disposed)
            {
                _disposed = true;
                if (calledFromDispose)
                {
                    Logger.Info("Disposing: Freeing managed DLL handles...");
                    foreach (IntPtr handle in _loadedDlls.Values)
                    {
                        FreeLibrary(handle);
                    }
                }
            }
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Attempts to locate a DLL file in the canonical location (subdirectory
        /// of the application directory) or, if set, in an alternative location.
        /// </summary>
        /// <param name="dllName">DLL to search</param>
        /// <returns>Complete path to the DLL file, or String.Empty if the DLL
        /// was not found.</returns>
        private string LocateDll(string dllName)
        {
            string dllPath = CompletePath(ApplicationDir(), dllName);
            bool found = File.Exists(dllPath);
            if (!found && !String.IsNullOrWhiteSpace(AlternativeDir))
            {
                dllPath = CompletePath(AlternativeDir, dllName);
                found = File.Exists(dllPath);
            }
            if (found)
            {
                return dllPath;
            }
            else
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// Constructs and returns the complete path for a given DLL.
        /// </summary>
        /// <remarks>
        /// By convention, the path that the DLL is expected to reside in is a subdirectory
        /// of the entry point assembly's base directory, in "bin/lib/$(Platform)", where
        /// $(Platform) can be "Win32" or "x64", for example.
        /// </remarks>
        /// <param name="fileName">Name of the DLL (with or without extension).</param>
        /// <returns>Path to the DLL subdirectory (platform-dependent).</returns>
        private string CompletePath(string baseDir, string fileName)
        {
            if (!Path.HasExtension(fileName))
            {
                fileName += ".dll";
            };
            string s = Path.Combine(baseDir,
                "lib",
                Environment.Is64BitProcess ? "x64" : "Win32",
                fileName);
            Logger.Info("Complete path: '{0}'", s);
            return s;
        }

        private string ApplicationDir()
        {
            return AppDomain.CurrentDomain.BaseDirectory;
        }

        private bool VerifyChecksum(string dllPath, string expectedSha1Hash)
        {
            return FileHelpers.Sha1Hash(dllPath) == expectedSha1Hash;
        }

        /// <summary>
        /// Sanitizes a DLL path by removing potentially sensitive information,
        /// i.e. the user name.
        /// </summary>
        /// <param name="dllPath">Path to sanitize</param>
        /// <returns></returns>
        private string SanitizeDllPath(string dllPath)
        {
            Logger.Info("Sanitizing DLL path:");
            Logger.Info("    {0}", dllPath);
            // Strip the leading directories from the path info (they may contain
            // sensitive information about where exactly a user has installed files).
            string[] dirs = Path.GetDirectoryName(dllPath).Split(Path.DirectorySeparatorChar);
            string gracefulPath = dllPath;
            int n = dirs.Length;
            if (n > 0) gracefulPath = Path.Combine(dirs[n - 1], gracefulPath);
            if (n > 1) gracefulPath = Path.Combine(dirs[n - 2], gracefulPath);
            if (n > 2) gracefulPath = Path.Combine("...", gracefulPath);
            Logger.Info("--> {0}", gracefulPath);
            return gracefulPath;
        }

        #endregion

        #region Class logger

        private static NLog.Logger Logger { get { return _logger.Value; } }

        private static readonly Lazy<NLog.Logger> _logger = new Lazy<NLog.Logger>(() => NLog.LogManager.GetCurrentClassLogger());

        #endregion
    }
}
