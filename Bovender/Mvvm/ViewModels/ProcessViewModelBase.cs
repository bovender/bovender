using Bovender.Mvvm.Messaging;
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

namespace Bovender.Mvvm.ViewModels
{
    /// <summary>
    /// Abstract base class for view models that deal with processes.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Note that this class does not deal with separate threads.
    /// Implementations may defer threading to a model or take care
    /// of it themselves.
    /// </para>
    /// </remarks>
    public abstract class ProcessViewModelBase : ViewModelBase
    {
        #region MVVM messages

        /// <summary>
        /// Message that signals that the process status may be displayed.
        /// This is sent a short while after StartProcess was called.
        /// This message is sent only once. Subsequent status updates
        /// are written to the shared ProcessMessageContent object.
        /// </summary>
        public Message<ProcessMessageContent> ShowProgress
        {
            get
            {
                if (_showProgress == null)
                {
                    _showProgress = new Message<ProcessMessageContent>();
                }
                return _showProgress;
            }
        }

        /// <summary>
        /// Message that signals if the process failed.
        /// </summary>
        public Message<StringMessageContent> ProcessFailedMessage
        {
            get
            {
                if (_processFailedMessage == null)
                {
                    _processFailedMessage = new Message<StringMessageContent>();
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
        {

        }

        #endregion

        #region Protected methods

        /// <summary>
        /// Entry point to starts the process. This method initializes the timer
        /// that updates the process status in regular intervals, then calls
        /// the Execute method followed by the EndProcess method.
        /// </summary>
        protected virtual void StartProcess()
        {
            _progressTimer = new Timer(UpdateProgress, null, 1000, 300);
            Execute();
            AfterStartProcess();
        }

        /// <summary>
        /// Additional work to do after the process has started.
        /// </summary>
        protected virtual void AfterStartProcess()
        {
            CloseViewCommand.Execute(null);
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
                    ShowProgress.Send(ProcessMessageContent);
                }
            }
            if (IsProcessing())
            {
                ProcessMessageContent.PercentCompleted = GetPercentCompleted();
            }
            else
            {
                _progressTimer.Dispose();
            }
        }
        
        #endregion

        #region Private fields

        private Message<ProcessMessageContent> _showProgress;
        private Message<StringMessageContent> _processFailedMessage;
        private ProcessMessageContent _processMessageContent;
        private Timer _progressTimer;
        private bool _showProgressWasSent;

        #endregion

        #region Class logger

        private static NLog.Logger Logger { get { return _logger.Value; } }

        private static readonly Lazy<NLog.Logger> _logger = new Lazy<NLog.Logger>(() => NLog.LogManager.GetCurrentClassLogger());

        #endregion

    }
}
