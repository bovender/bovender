/* ProcessCompletedAction.cs
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
using System.Windows;
using Bovender.Mvvm.Messaging;

namespace Bovender.Mvvm.Actions
{
    /// <summary>
    /// WPF action that invokes different views depending on the status
    /// of a completed process.
    /// </summary>
    public class ProcessCompletedAction : NotificationAction
    {
        #region Properties

        /// <summary>
        /// Is true if the Content is a ProcessMessageContent and has an
        /// Exception object.
        /// </summary>
        public bool HasException
        {
            get
            {
                ProcessMessageContent c = Content as ProcessMessageContent;
                return (c != null && c.Exception != null);
            }
        }

        #endregion

        #region Constructors

        public ProcessCompletedAction() : base() { }

        public ProcessCompletedAction(ProcessMessageContent processMessageContent)
            : this()
        {
            Content = processMessageContent;
        }

        public ProcessCompletedAction(
            ProcessMessageContent processMessageContent,
            string caption)
            : this(processMessageContent)
        {
            Caption = caption;
        }

        public ProcessCompletedAction(
            ProcessMessageContent processMessageContent,
            string caption,
            string message)
            : this(processMessageContent, caption)
        {
            Message = message;
        }

        public ProcessCompletedAction(
            ProcessMessageContent processMessageContent,
            string caption,
            string message,
            string okButtonText)
            : this(processMessageContent, caption, message)
        {
            OkButtonText = okButtonText;
        }

        public ProcessCompletedAction(
            ProcessMessageContent processMessageContent,
            string caption,
            string message,
            string okButtonText,
            string param)
            : this(processMessageContent, caption, message, okButtonText)
        {
            Param1 = param;
        }

        public ProcessCompletedAction(
            ProcessMessageContent processMessageContent,
            string caption,
            string message,
            string okButtonText,
            string param1, string param2)
            : this(processMessageContent, caption, message, okButtonText, param1)
        {
            Param2 = param2;
        }

        public ProcessCompletedAction(
            ProcessMessageContent processMessageContent,
            string caption,
            string message,
            string okButtonText,
            string param1, string param2, string param3)
            : this(processMessageContent, caption, message, okButtonText, param1, param2)
        {
            Param3 = param3;
        }

        #endregion

        #region Virtual methods

        protected virtual Window CreateSuccessWindow()
        {
            return new Bovender.Mvvm.Views.ProcessSucceededView();
        }

        protected virtual Window CreateFailureWindow()
        {
            return new Bovender.Mvvm.Views.ProcessFailedView();
        }

        protected virtual Window CreateCancelledWindow()
        {
            return new Bovender.Mvvm.Views.NotificationView();
        }

        #endregion

        #region Overrides

        protected override Window CreateView()
        {
            Logger.Info("ProcessCompletedAction.CreateView");
            ProcessMessageContent content = Content as ProcessMessageContent;
            if (Content is ProcessMessageContent)
            {
                if (content.WasCancelled)
                {
                    Logger.Info("Process was cancelled");
                    return CreateCancelledWindow();
                }
                else if (content.WasSuccessful)
                {
                    Logger.Info("Process was successful");
                    return CreateSuccessWindow();
                }
                else
                {
                    Logger.Warn("Process failed!");
                    return CreateFailureWindow();
                }
            }
            else
            {
                Logger.Fatal("ProcessCompletedAction requires ProcessMessageContent, got {0}",
                    Content.GetType().AssemblyQualifiedName);
                throw new ArgumentException(
                    "This message action must be used for Messages with ProcessMessageContent only.");
            }
        }

        #endregion

        #region Class logger

        private static NLog.Logger Logger { get { return _logger.Value; } }

        private static readonly Lazy<NLog.Logger> _logger = new Lazy<NLog.Logger>(() => NLog.LogManager.GetCurrentClassLogger());

        #endregion
    }
}
