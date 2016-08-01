using Bovender.Text;
/* ReleaseInfo.cs
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
using System.Net;
using System.Text;
using System.Threading;

namespace Bovender.Versioning
{
    /// <summary>
    /// Fetches and digests release information.
    /// </summary>
    public class ReleaseInfo : Mvvm.Models.ProcessModel, IReleaseInfo
    {
        #region Public properties

        /// <summary>
        /// Gets or sets the URI of the release info file.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This file must follow a simple, albeit specific format:
        /// </para>
        /// <list type="unordered"><
        /// <item>Line 1: Current release version; this must follow the semantic versioning
        /// scheme (https://semver.org).</item>
        /// <item>Line 2: URL for the installer. May contain $VERSION placeholders which
        /// will be substituted with the version from line 1.</item>
        /// <item>Line 3: Expected checksum of the release installer. This may be a SHA-1,
        /// SHA-256 or other checksum hash, depending on the length of the string.
        /// </item>
        /// <item>Lines 4-n: Release summary.</item>
        /// </list>
        /// </remarks>
        public Uri ReleaseInfoUri { get; protected set; }

        #endregion

        #region Implementation of IReleaseInfo

        public SemanticVersion ReleaseVersion { get; protected set; }

        public string Summary { get; protected set; }

        public Uri DownloadUri { get; protected set; }

        public string ExpectedHash { get; protected set; }

        public string RawReleaseInfo { get; protected set; }

        public ReleaseInfoStatus Status { get; protected set; }

        public Exception Exception { get; protected set; }

        public virtual bool Fetch()
        {
            bool result = false;
            if (ReleaseInfoUri == null)
            {
                Status = ReleaseInfoStatus.FailureToFetch;
                throw new InvalidOperationException("Cannot fetch release information, URI missing");
            }
            using (_webClient = new WebClient())
            {
                try
                {
                    _webClient.DownloadStringCompleted += WebClient_DownloadStringCompleted;
                    var lockObject = new Object();
                    lock (lockObject)
                    {
                        _webClient.DownloadStringAsync(ReleaseInfoUri, lockObject);
                        Monitor.Wait(lockObject);
                        result = true;
                    }
                }
                catch (Exception e)
                {
                    Exception = e;
                    SetDefaults();
                    Status = ReleaseInfoStatus.FailureToFetch;
                }
            }
            return result;
        }

        private void WebClient_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            lock (e.UserState)
            {
                Monitor.Pulse(e.UserState);
            }
            if (!e.Cancelled)
	        {
                if (e.Error == null)
                {
                    Logger.Info("WebClient_DownloadStringCompleted: Not cancelled and no error :-)");
                    RawReleaseInfo = e.Result;
                    Logger.Debug("WebClient_DownloadStringCompleted: \r\n{0}", RawReleaseInfo);
                    Status = Parse(RawReleaseInfo) ? ReleaseInfoStatus.InfoAvailable : ReleaseInfoStatus.FailureToParse;
                }
                else
                {
                    Logger.Warn("WebClient_DownloadStringCompleted: Exception occurred while fetching release info");
                    Logger.Warn(e.Error);
                    Status = ReleaseInfoStatus.FailureToFetch;
                    Exception = e.Error;
                }
            }
            else
            {
                Logger.Info("WebClient_DownloadStringCompleted: Fetching release info was cancelled");
                Status = ReleaseInfoStatus.InfoUnavailable;
            }
            Logger.Info("WebClient_DownloadStringCompleted: Status: {0}", Status);
        }

        #endregion

        #region Implementation of ProcessModel

        public override bool Execute()
        {
            return Fetch();
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new ReleaseInfo object that can fetch release information
        /// from the internet. The ReleaseInfoUri must be set separately.
        /// </summary>
        public ReleaseInfo() : base() { }

        /// <summary>
        /// Creates a new ReleaseInfo object that can fetch release information
        /// from the internet using the URI given as a parameter.
        /// </summary>
        public ReleaseInfo(Uri releaseInfoUri)
            : this()
        {
            ReleaseInfoUri = releaseInfoUri;
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

        #region Protected methods

        protected virtual bool Parse(string rawInfo)
        {
            bool result = false;
            using (StringReader r = new StringReader(rawInfo))
            {
                try
                {
                    ReleaseVersion = new SemanticVersion(r.ReadLine());
                    string rawUri = r.ReadLine();
                    // If the raw URI contains the placeholder $VERSION, replace
                    // it with the new version.
                    DownloadUri = new Uri(rawUri.Replace("$VERSION", ReleaseVersion.ToString()));
                    // Use only the first word of the line as Sha1 sum
                    // to make it compatible with the output of `sha1sum`
                    ExpectedHash = r.ReadLine().Trim().Split(' ')[0];
                    Multiline multi = new Multiline(r.ReadToEnd(), true);
                    Summary = multi.Text;
                    Status = ReleaseInfoStatus.InfoAvailable;
                    result = true;
                }
                catch (Exception e)
                {
                    SetDefaults();
                    Status = ReleaseInfoStatus.FailureToParse;
                    Exception = e;
                }
            }
            return result;
        }

        protected virtual void SetDefaults()
        {
            ReleaseVersion = null;
            DownloadUri = null;
            Summary = String.Empty;
            ExpectedHash = String.Empty;
        }

        #endregion

        #region Private fields

        private WebClient _webClient;

        #endregion

        #region Class logger

        private static NLog.Logger Logger { get { return _logger.Value; } }

        private static readonly Lazy<NLog.Logger> _logger = new Lazy<NLog.Logger>(() => NLog.LogManager.GetCurrentClassLogger());

        #endregion
    }
}
