/* ProcessAction.cs
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
using System.Windows;
using Bovender.Mvvm.Messaging;
using Bovender.Mvvm.Views;

namespace Bovender.Mvvm.Actions
{
    /// <summary>
    /// Invokes a process view and injects the ProcessMessageContent
    /// as a view model into it.
    /// </summary>
    /// <remarks>
    /// This action cannot inject itself into the view because actions
    /// are not view models by Bovender's definition. To enable a view
    /// that invoke this action to set the strings itself, the Caption
    /// and Message properties of the MessageActionBase parent class
    /// and a CancelButtonText are written to the message content
    /// object (if they are not null or empty strings) so that they
    /// are available in the newly created view that binds the message
    /// content as its view model.
    /// </remarks>
    public class ProcessAction : ShowViewAction
    {
        #region Overrides

        protected override System.Windows.Window CreateView()
        {
            Window view;
            // Attempt to create a view from the Assembly and View
            // parameters. If this fails, create a generic ProcessView.
            try
            {
                view = base.CreateView();
            }
            catch
            {
                view = new ProcessView();
            }
            // The ShowViewAction injects a ViewModelBase object into the view.
            // The ProcessAction returns a view with a ProcessMessageContent injected
            // into it.
            return view;
        }

        protected override ViewModels.ViewModelBase GetDataContext(MessageContent messageContent)
        {
            ProcessMessageContent pmc = messageContent as ProcessMessageContent;
            if (pmc == null)
            {
                Logger.Debug("ProcessMessageContent: Need ProcessMessageContent, got {0}",
                    messageContent == null ? "null" : messageContent.GetType().AssemblyQualifiedName);
                throw new ArgumentException("Cannot process because the message did not contain a ProcessMessageContent");
            }
            if (!string.IsNullOrEmpty(CancelButtonText))
            {
                Logger.Info("ProcessMessageContent: Overriding CancelButtonText");
                pmc.CancelButtonText = CancelButtonText;
            }
            return pmc;
        }

        #endregion

        #region Class logger

        private static NLog.Logger Logger { get { return _logger.Value; } }

        private static readonly Lazy<NLog.Logger> _logger = new Lazy<NLog.Logger>(() => NLog.LogManager.GetCurrentClassLogger());

        #endregion
    }
}
