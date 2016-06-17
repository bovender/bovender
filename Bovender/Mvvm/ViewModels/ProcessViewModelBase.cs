/* ProcessViewModelBase.cs
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
using System.Linq;
using System.Text;
using System.Threading;
using Bovender.Mvvm.Messaging;

namespace Bovender.Mvvm.ViewModels
{
    /// <summary>
    /// Abstract base class for view models that deal with processes.
    /// </summary>
    /// <remarks>
    /// <para>
    /// To show a progress bar, listen to the ProcessViewModelBase.ShowProgress message and
    /// create a Bovender.Mvvm.Views.ProcessView instance when it is sent:
    /// </para>
    /// <code>
    /// myProcessViewModel.ShowProgress.Sent += (sender, args) =>
    ///     {
    ///         args.Content.CancelButtonText = Strings.Cancel;
    ///         args.Content.Caption = Strings.ExportCsvFile;
    ///         args.Content.CompletedMessage.Sent += (sender2, args2) =>
    ///         {
    ///             args.Content.CloseViewCommand.Execute(null);
    ///         };
    ///         args.Content.InjectAndShowInThread&lt;Bovender.Mvvm.Views.ProcessView>()&gt;;
    ///     };
    /// </code>
    /// <para>
    /// When the process finishes successfully, the ProcessView will be
    /// closed automatically.
    /// </para>
    /// <para>
    /// If the process encounters an error, the ProcessViewModelBase.ProcessFailedMessage
    /// will be sent, and you may want to listen for it to inform the user:
    /// </para>
    /// <code>
    /// myProcessViewModel.ProcessFailedMessage.Sent += (sender, args) =>
    ///     {
    ///         MessageBox.Show(args.Content.Value, Strings.ExportCsvFile,
    ///             MessageBoxButton.OK, MessageBoxImage.Warning);
    ///     };
    /// </code>
    /// <para>
    /// Note that this class does not deal with separate threads.
    /// Implementations should defer threading to a model or take care
    /// of it themselves.
    /// </para>
    /// </remarks>
    public abstract class ProcessViewModelBase : ViewModelBase
    {
        #region Properties

        public Bovender.Mvvm.Models.ProcessModel ProcessModel
        {
            get
            {
                return _processModel;
            }
            set
            {
                if (_processModel != null)
                {
                    _processModel.ProcessFailed -= ProcessModel_ProcessFailed;
                    _processModel.ProcessSucceeded -= ProcessModel_ProcessSucceeded;
                }
                _processModel = value;
                if (_processModel != null)
                {
                    _processModel.ProcessFailed += ProcessModel_ProcessFailed;
                    _processModel.ProcessSucceeded += ProcessModel_ProcessSucceeded;
                }
            }
        }

        #endregion

        #region MVVM messages

        /// <summary>
        /// Message that signals that the process status may be displayed.
        /// This is sent a short while after StartProcess was called.
        /// This message is sent only once. Subsequent status updates
        /// are written to the shared ProcessMessageContent object.
        /// </summary>
        public Message<ProcessMessageContent> ShowProgressMessage
        {
            get
            {
                if (_showProgressMessage == null)
                {
                    _showProgressMessage = new Message<ProcessMessageContent>();
                }
                return _showProgressMessage;
            }
        }

        /// <summary>
        /// Message that signals when the process succeeded.
        /// </summary>
        public Message<ProcessMessageContent> ProcessSucceededMessage
        {
            get
            {
                if (_processSucceededMessage == null)
                {
                    _processSucceededMessage = new Message<ProcessMessageContent>();
                }
                return _processSucceededMessage;
            }
        }

        /// <summary>
        /// Message that signals when the process failed.
        /// </summary>
        public Message<ProcessMessageContent> ProcessFailedMessage
        {
            get
            {
                if (_processFailedMessage == null)
                {
                    _processFailedMessage = new Message<ProcessMessageContent>();
                }
                return _processFailedMessage;
            }
        }

        #endregion

        #region Abstract methods

        /// <summary>
        /// Core method that takes care of the actual process.
        /// </summary>
        protected abstract void Execute();

        /// <summary>
        /// Cancels the ongoing process.
        /// </summary>
        protected abstract void CancelProcess();

        /// <summary>
        /// Returns the percent completed value.
        /// </summary>
        /// <returns>Number between 0 and 100</returns>
        protected abstract int GetPercentCompleted();

        /// <summary>
        /// Returns true if the process is ongoing.
        /// </summary>
        /// <returns>True if the process is ongoing, otherwise false.</returns>
        protected abstract bool IsProcessing();

        #endregion

        #region Constructor

        protected ProcessViewModelBase()
            : base()
        { }

        #endregion

        #region Protected methods

        /// <summary>
        /// Entry point to starts the process. This method initializes the timer
        /// that updates the process status in regular intervals, then calls
        /// the Execute method followed by the EndProcess method.
        /// </summary>
        protected virtual void StartProcess()
        {
            Logger.Info("Starting process...");
            _progressTimer = new Timer(UpdateProgress, null, 1000, 300);
            Execute();
            AfterStartProcess();
        }

        /// <summary>
        /// Additional work to do after the process has started.
        /// </summary>
        protected virtual void AfterStartProcess()
        {
            Logger.Info("Additional work after starting the process");
            CloseViewCommand.Execute(null);
        }

        /// <summary>
        /// Sends the ProcessMessageContent.CompletedMessage to signal
        /// that the process has finished.
        /// </summary>
        protected virtual void SendProcessSucceededMessage()
        {
            Action action = new Action(() =>
            {
                Logger.Info("Sending ProcessSucceededMessage");
                ProcessSucceededMessage.Send();
                ProcessMessageContent.CompletedMessage.Send();
            });
            Dispatch(action);
        }

        /// <summary>
        /// Sends the ProcessFailedMessage to signal failure.
        /// Also writes the information to the Logger.
        /// </summary>
        protected virtual void SendProcessFailedMessage(Exception e)
        {
            Logger.Warn(e);
            SendProcessFailedMessage(e.Message);
        }

        /// <summary>
        /// Sends the ProcessFailedMessage to signal failure.
        /// Also writes the information to the Logger.
        /// </summary>
        protected virtual void SendProcessFailedMessage(string infoMessage)
        {
            Logger.Warn("Sending ProcessFailedMessage");
            ProcessFailedMessage.Send(ProcessMessageContent);
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Returns the associated Bovender.Mvvm.Models.ProcessModel (if any).
        /// </summary>
        /// <returns></returns>
        public override object RevealModelObject()
        {
            return ProcessModel;
        }

        #endregion

        #region Protected properties

        /// <summary>
        /// Message content for the process message. This is where the
        /// status updates occur. Process views get a hold of this
        /// message content when the ShowProcess message is sent.
        /// </summary>
        protected ProcessMessageContent ProcessMessageContent
        {
            get
            {
                if (_processMessageContent == null)
                {
                    _processMessageContent = new ProcessMessageContent(CancelProcess);
                }
                return _processMessageContent;
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Updates the process status. This is a callback method for
        /// the Timer that updates the process status in regular intervals.
        /// </summary>
        /// <param name="state"></param>
        private void UpdateProgress(object state)
        {
            if (!_showProgressWasSent)
            {
                _showProgressWasSent = true;
                if (IsProcessing())
                {
                    Dispatch((Action)(() =>
                    {
                        Logger.Info("UpdateProgress: Sending ShowProgressMessage");
                        ShowProgressMessage.Send(ProcessMessageContent);
                        Logger.Debug("UpdateProgress: ... ShowProgressMessage was sent");

                    }));

                }
            }
            if (IsProcessing())
            {
                int percent = GetPercentCompleted();
                Logger.Info("UpdateProgress: PercentCompleted: {0}", percent);
                ProcessMessageContent.PercentCompleted = percent;
            }
            else
            {
                Logger.Info("UpdateProgress: No longer processing, disposing update timer");
                _progressTimer.Dispose();
            }
        }

        private void ProcessModel_ProcessSucceeded(object sender, Models.ProcessModelEventArgs e)
        {
            // Because the ProcessModel may raise the event from a different thread,
            // we use the associated view's dispatcher (if any).
            Dispatch((Action)(() =>
            {
                SendProcessSucceededMessage();
            }));
        }

        private void ProcessModel_ProcessFailed(object sender, Models.ProcessModelEventArgs e)
        {
            Dispatch((Action)(() =>
            {
                SendProcessFailedMessage(e.ProcessException);
            }));
        }
        
        #endregion

        #region Private fields

        private Message<ProcessMessageContent> _showProgressMessage;
        private Message<ProcessMessageContent> _processSucceededMessage;
        private Message<ProcessMessageContent> _processFailedMessage;
        private ProcessMessageContent _processMessageContent;
        private Timer _progressTimer;
        private bool _showProgressWasSent;
        private Bovender.Mvvm.Models.ProcessModel _processModel;

        #endregion

        #region Class logger

        private static NLog.Logger Logger { get { return _logger.Value; } }

        private static readonly Lazy<NLog.Logger> _logger = new Lazy<NLog.Logger>(() => NLog.LogManager.GetCurrentClassLogger());

        #endregion

    }
}
