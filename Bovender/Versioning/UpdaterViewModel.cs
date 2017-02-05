/* UpdaterViewModel.cs
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
using System.Linq;
using System.Text;
using Bovender.Mvvm;
using Bovender.Mvvm.ViewModels;
using Bovender.Mvvm.Messaging;
using System.Diagnostics;

namespace Bovender.Versioning
{
    public class UpdaterViewModel : ProcessViewModelBase
    {
        #region Public properties

        public double DownloadMegaBytesReceived
        {
            get
            {
                return Updater.DownloadBytesReceived / 1000000;
            }
        }

        public double DownloadMegaBytesTotal
        {
            get
            {
                return Updater.DownloadBytesTotal / 1000000;
            }
        }

        public SemanticVersion CurrentVersion { get { return Updater.CurrentVersion; } }

        public SemanticVersion NewVersion { get { return Updater.ReleaseInfo.ReleaseVersion; } }

        public string DownloadFolder { get; set; }

        public string Summary { get { return Updater.ReleaseInfo.Summary; } }

        #endregion

        #region MVVM commands

        public DelegatingCommand DownloadCommand
        {
            get
            {
                if (_downloadCommand == null)
                {
                    _downloadCommand = new DelegatingCommand(
                        param => StartProcess(),
                        param => CanStartProcess());
                }
                return _downloadCommand;
            }
        }

        public DelegatingCommand ChooseFolderCommand
        {
            get
            {
                if (_chooseFolderCommand == null)
                {
                    _chooseFolderCommand = new DelegatingCommand(ChooseFolder);
                }
                return _chooseFolderCommand;
            }
        }

        public DelegatingCommand InstallCommand
        {
            get
            {
                if (_installCommand == null)
                {
                    _installCommand = new DelegatingCommand(Install);
                }
                return _installCommand;
            }
        }

        #endregion

        #region MVVM messages

        public Message<FileNameMessageContent> ChooseFolderMessage
        {
            get
            {
                if (_chooseFolderMessage == null)
                {
                    _chooseFolderMessage = new Message<FileNameMessageContent>();
                }
                return _chooseFolderMessage;
            }
        }

        public Message<ViewModelMessageContent> DownloadFinishedMessage
        {
            get
            {
                if (_downloadFinishedMessage == null)
                {
                    _downloadFinishedMessage = new Message<ViewModelMessageContent>();
                }
                return _downloadFinishedMessage;
            }
        }

        public Message<ViewModelMessageContent> DownloadFailedMessage
        {
            get
            {
                if (_downloadFailedMessage == null)
                {
                    _downloadFailedMessage = new Message<ViewModelMessageContent>();
                }
                return _downloadFailedMessage;
            }
        }

        #endregion

        #region Constructors

        public UpdaterViewModel(Updater updater)
            : base(updater)
        { }
        
        #endregion

        #region Implementation of ProcessViewModelBase

        protected override void UpdateProcessMessageContent(ProcessMessageContent processMessageContent)
        {
            DownloadProcessMessageContent d = processMessageContent as DownloadProcessMessageContent;
            processMessageContent.PercentCompleted = Updater.PercentDownloaded;
            if (d != null)
	        {
                d.DownloadMegaBytesReceived = (double)Updater.DownloadBytesReceived / 1000000;
                d.DownloadMegaBytesTotal = (double)Updater.DownloadBytesTotal / 1000000;
	        }
            else
            {
                Logger.Warn("UpdateProcessMessageContent: processMessageContent is not a DownloadProcessMessageContent!");
            }
        }
        
        #endregion

        #region Protected methods

        protected virtual void SendDownloadFinishedMessage()
        {
            Logger.Info("SendDownloadFinishedMessage");
            DownloadFinishedMessage.Send(new ViewModelMessageContent(this));
        }

        protected virtual void SendDownloadFailedMessage()
        {
            Logger.Info("SendDownloadFailedMessage");
            DownloadFailedMessage.Send(new ViewModelMessageContent(this));
        }

        protected virtual bool CanStartProcess()
        {
            return !IsProcessing;
        }

        protected virtual void ChooseFolder(object param)
        {
            ChooseFolderMessage.Send(new FileNameMessageContent(DownloadFolder), ConfirmFolder);
        }

        protected virtual void ConfirmFolder(FileNameMessageContent fileNameMessageContent)
        {
            if (fileNameMessageContent.Confirmed)
            {
                Logger.Info("ConfirmFolder: Confirmed, proceeding to start download");
                Updater.DestinationFolder = fileNameMessageContent.Value;
                StartProcess();
            }
            else
            {
                Logger.Info("ConfirmFolder: Not confirmed!");
            }
        }

        protected virtual void Install(object param)
        {
            DoCloseView();
            Updater.Install();
        }

        #endregion

        #region Protected properties

        protected Updater Updater
        {
            [DebuggerStepThrough]
            get
            {
                return (Updater)ProcessModel;
            }
        }

        #endregion

        #region Overrides

        protected override void SendProcessFinishedMessage()
        {
            base.SendProcessFinishedMessage();
            switch (Updater.Status)
            {
                case UpdaterStatus.Downloaded:
                    DownloadFinishedMessage.Send(ProcessMessageContent);
                    break;
                case UpdaterStatus.DownloadFailed:
                    DownloadFailedMessage.Send(ProcessMessageContent);
                    break;
                default:
                    break;
            }
        }

        protected override ProcessMessageContent ProcessMessageContent
        {
            get
            {
                if (_downloadProcessMessageContent == null)
                {
                    _downloadProcessMessageContent = new DownloadProcessMessageContent(this, CancelProcess);
                }
                return _downloadProcessMessageContent;
            }
        }
       
        #endregion

        #region Fields

        private DownloadProcessMessageContent _downloadProcessMessageContent;
        private DelegatingCommand _downloadCommand;
        private DelegatingCommand _chooseFolderCommand;
        private DelegatingCommand _installCommand;
        private Message<ViewModelMessageContent> _downloadFinishedMessage;
        private Message<ViewModelMessageContent> _downloadFailedMessage;
        private Message<FileNameMessageContent> _chooseFolderMessage;

        #endregion

        #region Class logger

        private static NLog.Logger Logger { get { return _logger.Value; } }

        private static readonly Lazy<NLog.Logger> _logger = new Lazy<NLog.Logger>(() => NLog.LogManager.GetCurrentClassLogger());

        #endregion
    }
}
