/* DllManager.cs
 * part of Daniel's XL Toolbox NG
 * 
 * Copyright 2014-2018 Daniel Kraus
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
    public class DllManager : IDisposable
    {
        #region Public static property

        public static string AlternativeDir { get; set; }

        #endregion

        #region Public static methods

        /// <summary>
        /// Checks whether a DLL exists in the application directory or
        /// in the custom <see cref="AlternativeDir"/>.
        /// </summary>
        /// <param name="dllFileName">DLL file name to check</param>
        /// <returns>True if the file exists at either of the two locations; 
        /// false if not.</returns>
        public static bool DllExists(string dllFileName)
        {
            Logger.Info("DllExists: {0}", dllFileName);
            return !String.IsNullOrEmpty(LocateDll(dllFileName));
        }

        /// <summary>
        /// Attempts to locate a DLL file in the canonical location (subdirectory
        /// of the application directory) or, if set, in an alternative location.
        /// </summary>
        /// <param name="dllName">DLL to search</param>
        /// <returns>Complete path to the DLL file, or String.Empty if the DLL
        /// was not found.</returns>
        public static string LocateDll(string dllName)
        {
            Logger.Info("LocateDll: {0}", dllName);
            string dllPath = CompletePath(ApplicationDir(), dllName);
            bool found = File.Exists(dllPath);
            if (!found && !String.IsNullOrWhiteSpace(AlternativeDir))
            {
                Logger.Info("LocateDll: Looking in alternative directory '{0}'", AlternativeDir);
                dllPath = CompletePath(AlternativeDir, dllName);
                found = File.Exists(dllPath);
            }
            if (found)
            {
                Logger.Info("LocateDll: Found");
                return dllPath;
            }
            else
            {
                Logger.Warn("LocateDll: {0} not found", dllName);
                return String.Empty;
            }
        }

        #endregion

        #region Private static methods

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
        private static string CompletePath(string baseDir, string fileName)
        {
            if (!Path.HasExtension(fileName))
            {
                fileName += ".dll";
            };
            string s = Path.Combine(baseDir,
                "lib",
                Environment.Is64BitProcess ? "x64" : "Win32",
                fileName);
            Logger.Info("CompletePath: '{0}'", s);
            return s;
        }

        private static string ApplicationDir()
        {
            return AppDomain.CurrentDomain.BaseDirectory;
        }
        
        #endregion

        #region Public methods

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
        /// <param name="expectedSha256Hash">Expected Sha1 hash of the DLL.</param>
        /// <exception cref="DllNotFoundException">if the file is not found in the path.</exception>
        /// <exception cref="DllLoadingFailedException">if the file could not be loaded.</exception>
        /// <exception cref="DllSha1MismatchException">if the file's Sha1 is unexpected.</exception>
        // TODO: Use two expected hashes, one for Win32, one for x64
        public void LoadDll(string dllName, string expectedSha256Hash)
        {
            if (_dlls.Contains(dllName))
            {
                Logger.Info("LoadDll: '{0}' already registered in this instance", dllName);
            }
            else
            {
                Logger.Info("LoadDll: '{0}'", dllName);

                string dllPath = LocateDll(dllName);
                if (String.IsNullOrEmpty(dllPath))
                {
                    Logger.Fatal("LoadDll: Unable to locate DLL!");
                    throw new DllNotFoundException(String.Format("Unable to locate DLL file"));
                }

                DllFile dllFile;
                if (_globalDlls.TryGetValue(dllName, out dllFile))
                {
                    Logger.Info("LoadDll: Already registered");
                }
                else
                {
                    Logger.Info("LoadDll: Registering new DLL");
                    dllFile = new DllFile(dllPath, expectedSha256Hash);
                    _globalDlls.Add(dllName, dllFile);
                }
                dllFile.Load();
                _dlls.Add(dllName);
            }
        }

        /// <summary>
        /// Unloads a previously loaded DLL. Does nothing if the DLL was not loaded.
        /// </summary>
        /// <param name="dllName">Name of the DLL to unload.</param>
        public void UnloadDll(string dllName)
        {
            if (_dlls.Contains(dllName))
            {
                DllFile dllFile;
                if (_globalDlls.TryGetValue(dllName, out dllFile))
                {
                    Logger.Info("UnloadDll: Unloading '{0}'", dllName);
                    // Decrease the usage counter
                    dllFile.Unload();
                    _globalDlls.Remove(dllName);
                }
                else
                {
                    Logger.Warn("UnloadDll: Attempting to unload '{0}' which is not globally registered?!", dllName);
                }
                _dlls.Remove(dllName);
            }
            else
            {
                Logger.Warn("UnloadDll: '{0}' not known to this instance");
            }
        }

        #endregion

        #region Constructor

        public DllManager()
        {
            _dlls = new List<string>();
        }

        public DllManager(params string[] loadDlls)
            : this()
        {
            Logger.Info("DllManager: Constructor invoked with loadDlls array");
            foreach (string dll in loadDlls)
            {
                LoadDll(dll);
            }
            Logger.Info("DllManager: All loaded");
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
            if (!_disposed)
            {
                _disposed = true;
                if (calledFromDispose)
                {
                    // May use managed resources here
                    Logger.Info("Dispose: Unloading {0} DLL(s).", _dlls.Count);
                    foreach (string dll in _dlls)
                    {
                        DllFile dllFile;
                        if (_globalDlls.TryGetValue(dll, out dllFile))
                        {
                            dllFile.Unload();
                        }
                    }
                }
            }
        }

        #endregion

        #region Private fields

        private List<string> _dlls;
        private bool _disposed;

        #endregion

        #region Private static fields

        /// <summary>
        /// Holds the currently loaded DLL names and their handles.
        /// </summary>
        private static readonly Dictionary<string, DllFile> _globalDlls = new Dictionary<string, DllFile>();

        #endregion

        #region Private constant

        private const string LIBDIR = "lib";

        #endregion

        #region Class logger

        private static NLog.Logger Logger { get { return _logger.Value; } }

        private static readonly Lazy<NLog.Logger> _logger = new Lazy<NLog.Logger>(() => NLog.LogManager.GetCurrentClassLogger());

        #endregion
    }
}
