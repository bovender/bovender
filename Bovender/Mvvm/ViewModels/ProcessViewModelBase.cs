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
using System.Threading.Tasks;

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

        public Bovender.Mvvm.Models.ProcessModel ProcessModel { get; protected set; }

        public bool IsProcessing
        {
            get
            {
                return ProcessMessageContent.Processing;
            }
        }

        public bool WasCancelled
        {
            get
            {
                return ProcessMessageContent.WasCancelled;
            }
        }

        public bool WasSuccessful
        {
            get
            {
                return ProcessMessageContent.WasSuccessful;
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Entry point to starts the process. This method initializes the timer
        /// that updates the process status in regular intervals, then calls
        /// the Execute method followed by the EndProcess method.
        /// </summary>
        public virtual void StartProcess()
        {
            Logger.Info("Starting process...");
            _progressTimer = new Timer(
                UpdateProgress,
                null,
                Properties.Settings.Default.ShowProgressDelay,
                Properties.Settings.Default.ShowProgressInterval);
            Execute();
        }

        /// <summary>
        /// Cancels the ongoing process.
        /// </summary>
        public virtual void CancelProcess()
        {
            Logger.Info("CancelProcess was called!");
            ProcessMessageContent.WasCancelled = true;
            ProcessModel.Cancel();
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
        public Message<ProcessMessageContent> ProcessFinishedMessage
        {
            get
            {
                if (_processFinishedMessage == null)
                {
                    _processFinishedMessage = new Message<ProcessMessageContent>();
                }
                return _processFinishedMessage;
            }
        }

        #endregion

        #region Abstract methods

        /// <summary>
        /// Returns the percent completed value.
        /// </summary>
        /// <returns>Number between 0 and 100</returns>
        protected abstract int GetPercentCompleted();

        #endregion

        #region Constructor

        protected ProcessViewModelBase(Models.ProcessModel processModel)
            : base()
        {
            ProcessModel = processModel;
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// Additional work to do before the process is started.
        /// This will be called synchronously.
        /// </summary>
        /// <returns>True or false. If true, the process is started.
        /// If false, the process is not started.</returns>
        protected virtual bool BeforeStartProcess()
        {
            Logger.Info("Additional work before starting the process");
            return true;
        }

        /// <summary>
        /// Additional work to do after the process has started.
        /// In the base implementation, this executes the
        /// CloseViewCommand.
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
        protected virtual void SendProcessFinishedMessage()
        {
            Logger.Info("Sending ProcessFinishedMessage");
            ProcessMessageContent.Processing = false;
            ProcessMessageContent.Exception = Exception;
            ProcessFinishedMessage.Send(ProcessMessageContent);
            ProcessMessageContent.CompletedMessage.Send(ProcessMessageContent);
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
                    Logger.Debug("Creating new ProcessMessageContent instance");
                    _processMessageContent = new ProcessMessageContent(CancelProcess);
                }
                return _processMessageContent;
            }
        }

        protected Exception Exception { get; set; }

        #endregion

        #region Private methods

        /// <summary>
        /// Core method that takes care of the actual process. In the
        /// base implementation, this calls the ProcessModelBase.Execute
        /// method in a dedicated task. Upon completion of the task,
        /// the ProcessSucceededMessage or the ProcessFailedMessage are
        /// sent in the original synchronization context.
        /// </summary>
        /// <exception cref="InvalidOperationException">if this method
        /// is called while the process is already running.</exception>
        private void Execute()
        {
            if (ProcessMessageContent.Processing)
            {
                throw new InvalidOperationException("Cannot start the process because it is already running");
            }
            if (!BeforeStartProcess()) return;

            ProcessMessageContent.Processing = true;
            ProcessMessageContent.WasSuccessful = false;
            ProcessMessageContent.WasCancelled = false;
            Task.Factory.StartNew((Action)(() =>
            {
                try
                {
                    ProcessMessageContent.WasSuccessful = ProcessModel.Execute();
                }
                catch (Exception e)
                {
                    Exception = e;
                }
            })).ContinueWith((task) => Dispatch(SendProcessFinishedMessage));
            AfterStartProcess();
        }

        /// <summary>
        /// Updates the process status. This is a callback method for
        /// the Timer that updates the process status in regular intervals.
        /// </summary>
        /// <param name="state"></param>
        private void UpdateProgress(object state)
        {
            lock (_lockUpdateProgress)
            {
                if (!_showProgressWasSent)
                {
                    if (ProcessMessageContent.Processing)
                    {
                        _showProgressWasSent = true;
                        Dispatch(() =>
                        {
                            Logger.Info("UpdateProgress: Sending ShowProgressMessage");
                            ShowProgressMessage.Send(ProcessMessageContent);
                            Logger.Debug("UpdateProgress: ... ShowProgressMessage was sent");
                        });
                    }
                }
                else
                {
                    if (ProcessMessageContent.Processing)
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
            }
        }

        #endregion

        #region Private fields

        private Message<ProcessMessageContent> _showProgressMessage;
        private Message<ProcessMessageContent> _processFinishedMessage;
        private ProcessMessageContent _processMessageContent;
        private Timer _progressTimer;
        private bool _showProgressWasSent;

        #endregion

        #region Private static fields
        
        private static readonly object _lockUpdateProgress = new object();
        
        #endregion

        #region Class logger

        private static NLog.Logger Logger { get { return _logger.Value; } }

        private static readonly Lazy<NLog.Logger> _logger = new Lazy<NLog.Logger>(() => NLog.LogManager.GetCurrentClassLogger());

        #endregion
    }
}
