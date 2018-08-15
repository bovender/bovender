/* Updater.cs
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
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using Bovender.Mvvm;
using Bovender.Mvvm.Messaging;
using Bovender.Text;
using System.Threading;

namespace Bovender.Versioning
{
    /// <summary>
    /// Fetches version information from the internet and raises an UpdateAvailable
    /// event if a new version is available for download.
    /// </summary>
    /// <remarks>
    /// The current version information resides in a simple text file which contains
    /// four lines:              e.g.
    /// 1) Current version       7.0.0-alpha.1
    /// 2) Download URL          http://sourceforge.net/projects/xltoolbox/files/XL_Toolbox_7.0.0-alpha.1.exe
    /// 3) Sha1 of executable    1234abcd...
    /// 4) Version description   This is the first release of the next generation Toolbox
    /// </remarks>
    public class Updater : Mvvm.Models.ProcessModel, IDisposable
    {
        #region Public properties

        public UpdaterStatus Status { get; protected set;  }

        public Exception Exception { get; protected set; }

        public IReleaseInfo ReleaseInfo { get; set; }

        public string DestinationFolder { get; set; }

        public SemanticVersion CurrentVersion { get; set; }

        public int PercentDownloaded { get; protected set; }

        public long DownloadBytesTotal { get; protected set; }

        public long DownloadBytesReceived { get; protected set; }

        #endregion

        #region Public methods

        public bool Install()
        {
            bool result = false;
            if (Status == UpdaterStatus.Downloaded)
            {
                if (Verify())
                {
                    if (CheckAuthorization())
                    {
                        string command = GetInstallerCommand();
                        string arguments = GetInstallerParameters();
                        try
                        {
                            Logger.Info("Install: {0} {1}", command, arguments);
                            var process = System.Diagnostics.Process.Start(command, arguments);
                            Logger.Info("Install: Process: {0}", process);
                            Status = UpdaterStatus.InstallationStarted;
                            result = true;
                        }
                        catch (Exception e)
                        {
                            Logger.Fatal("Install: Could not start process");
                            Logger.Fatal(e);
                            Status = UpdaterStatus.InstallationFailed;
                            Exception = e;
                        }
                    }
                    else
                    {
                        Logger.Warn("Install: Not authorized to install!");
                        Status = UpdaterStatus.NotAuthorizedToInstall;
                    }
                }
                else
                {
                    Logger.Warn("Install: Verification failed");
                    Status = UpdaterStatus.VerificationFailed;
                }
            }
            else
            {
                Logger.Warn("Install: No update available!");
            }
            return result;
        }

        #endregion

        #region Constructors

        public Updater() : base() { }

        public Updater(IReleaseInfo releaseInfo)
            : this()
        {
            ReleaseInfo = releaseInfo;
        }

        #endregion

        #region Disposal

        ~Updater()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;
                if (disposing)
                {
                    // May clean up managed resources
                    if (_webClient != null)
                    {
                        _webClient.Dispose();
                    }
                }
            }
        }

        #endregion

        #region Implementation of ProcessModel

        public override bool Execute()
        {
            if (ReleaseInfo == null)
            {
                throw new InvalidOperationException("Cannot download release because ReleaseInfo is null");
            }
            if (ReleaseInfo.Status != ReleaseInfoStatus.InfoAvailable)
            {
                throw new InvalidOperationException("Cannot download release because no release info is available");
            }
            bool result = false;
            _destinationFileName = GetDestinationFileName();

            if (!(File.Exists(_destinationFileName) && Verify()))
            {
                Logger.Info("Execute: Starting download");
                // http://stackoverflow.com/a/25834736/270712
                _webClient = new WebClient();
                _webClient.DownloadFileCompleted += WebClient_DownloadFileCompleted;
                _webClient.DownloadProgressChanged += WebClient_DownloadProgressChanged;
                var lockObject = new Object();
                lock (lockObject)
                {
                    _webClient.DownloadFileAsync(ReleaseInfo.DownloadUri, _destinationFileName, lockObject);
                    Monitor.Wait(lockObject);
                }
                result = true;
                Logger.Info("Execute: exiting");
            }
            else
            {
                Logger.Info("Execute: Skipping download because destination file exists and is verified");
                Status = UpdaterStatus.Downloaded;
            }
            return result;
        }

	    #endregion

        #region Protected helper methods

        /// <summary>
        /// Builds the destination file name from the download URI
        /// and the destination folder (which is stored in a public property
        /// and could be set by a view that subscribes to this view model).
        /// </summary>
        /// <remarks>
        /// Derived classes will typically want to override this, as
        /// the base method uses a simple generic file name that contains
        /// the version number.
        /// </remarks>
        /// <returns>Complete path of the destination file.</returns>
        protected virtual string GetDestinationFileName()
        {
            string downloadUri = ReleaseInfo.DownloadUri.ToString();
            Logger.Info("GetDestinationFileName: Examining {0}", downloadUri);
            string fn;
            Regex r = new Regex(@"(?<fn>[^/]+?\.exe)");
            Match m = r.Match(downloadUri);
            if (m.Success)
            {
                fn = m.Groups["fn"].Value;
            }
            else
            {
                Logger.Warn("GetDestinationFileName: Did not find file name pattern in download URI!");
                fn = String.Format("release-{0}.exe", ReleaseInfo.ReleaseVersion.ToString());
            };
            Logger.Warn("GetDestinationFileName: {0}", fn);
            if (String.IsNullOrEmpty(DestinationFolder))
            {
                Logger.Warn("GetDestinationFileName: No destination folder!");
                return fn;
            }
            else
            {
                return Path.Combine(DestinationFolder, fn);
            }
        }

        /// <summary>
        /// Returns the command to execute in the shell to install the update.
        /// </summary>
        /// <returns>Command to execute.</returns>
        protected virtual string GetInstallerCommand()
        {
            return _destinationFileName;
        }

        /// <summary>
        /// Returns commandline parameters for the update installer.
        /// </summary>
        protected virtual string GetInstallerParameters()
        {
            // silencing parameters for InnoSetup installers
            return "/UPDATE /SP- /SILENT /SUPPRESSMSGBOXES";
        }

        /// <summary>
        /// Determines whether the current user is authorized to write to the folder
        /// where the addin files are stored. If the user does not have write permissions,
        /// he/she cannot update the addin by herself/hisself.
        /// </summary>
        protected virtual bool CheckAuthorization()
        {
            string installFolder = AppDomain.CurrentDomain.BaseDirectory;
            /* Todo: compute permissions, rather than try and catch */
            try
            {
                Logger.Info("CheckAuthorization: Attempting write to assembly's folder");
                Logger.Info("CheckAuthorization: {0}", installFolder);
                string fn = Path.Combine(installFolder, "bovender-framework-check-auth.txt");
                using (FileStream f = new FileStream(fn, FileMode.Create, FileAccess.Write))
                {
                    f.WriteByte(0xff);
                };
                File.Delete(fn);
                Logger.Warn("CheckAuthorization: Successfully created and deleted test file");
                return true;
            }
            catch (Exception e)
            {
                Logger.Warn("CheckAuthorization: Evidently not authorized");
                Logger.Warn(e);
                return false;
            }
        }

        /// <summary>
        /// Compares the actual and expected checksum hashes and sets
        /// the IsVerifiedDownload property accordingly. Automatically
        /// switches between SHA-256 and the obsolete SHA-1 algorithms.
        /// </summary>
        protected virtual bool Verify()
        {
            string actual;
            bool verified = false;
            Logger.Info("Verify: Expected: {0}", ReleaseInfo.ExpectedHash);
            switch (ReleaseInfo.ExpectedHash.Length)
            {
                case 40:
                    actual = FileHelpers.Sha1Hash(_destinationFileName); break;
                case 64:
                    actual = FileHelpers.Sha256Hash(_destinationFileName); break;
                default:
                    return false;
            }
            Logger.Info("Verify: Actual:   {0}", actual);
            verified = actual == ReleaseInfo.ExpectedHash;
            if (verified)
            {
                Logger.Info("Verify: OK");
            }
            else
            {
                Logger.Warn("Verify: Checksum mismacth!");
            }
            return verified;
        }

        #endregion

        #region Overrides

        protected override void OnCancelling()
        {
            base.OnCancelling();
            if (_webClient != null && _webClient.IsBusy)
            {
                _webClient.CancelAsync();
            }
        }

        #endregion

        #region Private event handlers

        private void WebClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs args)
        {
            PercentDownloaded = args.ProgressPercentage;
            DownloadBytesReceived = args.BytesReceived;
            DownloadBytesTotal = args.TotalBytesToReceive;
        }

        private void WebClient_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs args)
        {
            lock (args.UserState)
            {
                Monitor.Pulse(args.UserState);
            }
            if (args.Cancelled)
            {
                Logger.Info("WebClient_DownloadFileCompleted: Cancelled");
                try
                {
                    System.IO.File.Delete(_destinationFileName);
                    Logger.Info("WebClient_DownloadFileCompleted: Deleted partially downloaded file");
                }
                catch (Exception e)
                {
                    Logger.Warn("WebClient_DownloadFileCompleted: Could not remove partially downloaded file");
                    Logger.Warn(e);
                }
                Status = UpdaterStatus.DownloadCancelled;
            }
            else
            {
                if (args.Error == null)
                {
                    Logger.Info("WebClient_DownloadFileCompleted: Downloaded");
                    Status = UpdaterStatus.Downloaded;
                }
                else
                {
                    Logger.Warn("WebClient_DownloadFileCompleted: Error!");
                    Logger.Warn(args.Error);
                    Status = UpdaterStatus.DownloadFailed;
                    Exception = args.Error;
                }
            }
        }

        #endregion

        #region Private fields

        private bool _disposed;
        private WebClient _webClient;
        private string _destinationFileName;

        #endregion

        #region Class logger

        private static NLog.Logger Logger { get { return _logger.Value; } }

        private static readonly Lazy<NLog.Logger> _logger = new Lazy<NLog.Logger>(() => NLog.LogManager.GetCurrentClassLogger());

        #endregion
    }
}
