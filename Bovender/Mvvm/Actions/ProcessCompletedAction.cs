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
    /// Abstract WPF action that invokes different views depending on the status
    /// of a completed process.
    /// </summary>
    public abstract class ProcessCompletedAction : MessageActionBase
    {
        #region Abstract methods

        protected abstract Window CreateSuccessWindow();
        protected abstract Window CreateFailureWindow();
        protected abstract Window CreateCancelledWindow();

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
                    return content.InjectInto(CreateCancelledWindow());
                }
                else if (content.WasSuccessful)
                {
                    Logger.Info("Process was successful");
                    return content.InjectInto(CreateSuccessWindow());
                }
                else
                {
                    Logger.Warn("Process failed!");
                    return content.InjectInto(CreateFailureWindow());
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
