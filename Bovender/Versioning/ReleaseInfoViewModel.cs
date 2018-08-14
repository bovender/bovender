/* ReleaseInfoViewModel.cs
 * part of Bovender framework
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
using Bovender.Mvvm.ViewModels;
using Bovender.Mvvm.Messaging;
using Bovender.Mvvm;

namespace Bovender.Versioning
{
    /// <summary>
    /// A view model for the IReleaseInfo interface; i.e. update checker.
    /// </summary>
    public class ReleaseInfoViewModel : ProcessViewModelBase
    {
        #region Public properties

        public SemanticVersion ReleaseVersion { get { return ReleaseInfo.ReleaseVersion; } }

        public SemanticVersion CurrentVersion { get; set; }

        public string Summary { get { return ReleaseInfo.Summary; } }

        public Uri DownloadUri { get { return ReleaseInfo.DownloadUri; } }

        public string ExpectedHash { get { return ReleaseInfo.ExpectedHash; } }

        public string RawReleaseInfo { get { return ReleaseInfo.RawReleaseInfo; } }

        public ReleaseInfoStatus Status { get { return ReleaseInfo.Status; } }

        public override Exception Exception { get { return ReleaseInfo.Exception; } }

        #endregion

        #region MVVM commands

        public DelegatingCommand CheckForUpdateCommand
        {
            get
            {
                if (_checkForUpdateCommand == null)
                {
                    _checkForUpdateCommand = new DelegatingCommand(
                        (param) => StartProcess());
                }
                return _checkForUpdateCommand;
            }
        }
        
        #endregion

        #region MVVM messages

        public Message<ProcessMessageContent> UpdateAvailableMessage
        {
            get
            {
                if (_updateAvailableMessage == null)
                {
                    _updateAvailableMessage = new Message<ProcessMessageContent>();
                }
                return _updateAvailableMessage;
            }
        }

        public Message<ProcessMessageContent> NoUpdateAvailableMessage
        {
            get
            {
                if (_noUpdateAvailableMessage == null)
                {
                    _noUpdateAvailableMessage = new Message<ProcessMessageContent>();
                }
                return _noUpdateAvailableMessage;
            }
        }

        public Message<ProcessMessageContent> ExceptionMessage
        {
            get
            {
                if (_exceptionMessage == null)
                {
                    _exceptionMessage = new Message<ProcessMessageContent>();
                }
                return _exceptionMessage;
            }
        }

        #endregion

        #region Constructors

        public ReleaseInfoViewModel(SemanticVersion currentVersion)
            : this(new ReleaseInfo(), currentVersion)
        { }

        public ReleaseInfoViewModel(ReleaseInfo releaseInfo, SemanticVersion currentVersion)
            : base(releaseInfo)
        {
            CurrentVersion = currentVersion;
        }

        #endregion

        #region Implementation and overrides of ProcessViewModelBase

        protected override void UpdateProcessMessageContent(ProcessMessageContent processMessageContent)
        {
            processMessageContent.IsIndeterminate = true;
        }

        protected override void SendProcessFinishedMessage()
        {
            switch (Status)
            {
                case ReleaseInfoStatus.InfoAvailable:
                    ProcessMessageContent.WasSuccessful = true;
                    if (ReleaseVersion > CurrentVersion)
                    {
                        Logger.Info("SendProcessFinishedMessage: update available, {0} > {1}",
                            ReleaseVersion.ToString(), CurrentVersion.ToString());
                        UpdateAvailableMessage.Send(ProcessMessageContent);
                    }
                    else
                    {
                        Logger.Info("SendProcessFinishedMessage: no update available, {0} <= {1}",
                            ReleaseVersion.ToString(), CurrentVersion.ToString());
                        NoUpdateAvailableMessage.Send(ProcessMessageContent);
                    }
                    break;
                case ReleaseInfoStatus.FailureToFetch: 
                case ReleaseInfoStatus.FailureToParse:
                    Logger.Warn("SendProcessFinishedMessage: Exception occurred!");
                    Logger.Warn(ReleaseInfo.Exception);
                    ProcessMessageContent.Exception = ReleaseInfo.Exception;
                    ProcessMessageContent.WasSuccessful = false;
                    ExceptionMessage.Send(ProcessMessageContent);
                    break;
                default:
                    break;
            }
            base.SendProcessFinishedMessage();
        }

        #endregion

        #region Protected properties

        protected IReleaseInfo ReleaseInfo
        {
            get
            {
                return (IReleaseInfo)ProcessModel;
            }
        }

        #endregion

        #region Fields

        private DelegatingCommand _checkForUpdateCommand;
        private Message<ProcessMessageContent> _updateAvailableMessage;
        private Message<ProcessMessageContent> _noUpdateAvailableMessage;
        private Message<ProcessMessageContent> _exceptionMessage;
		 
	    #endregion

        #region Class logger

        private static NLog.Logger Logger { get { return _logger.Value; } }

        private static readonly Lazy<NLog.Logger> _logger = new Lazy<NLog.Logger>(() => NLog.LogManager.GetCurrentClassLogger());

        #endregion
    }
}
