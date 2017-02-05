/* ShowViewAction.cs
 * part of Daniel's XL Toolbox NG
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
using System.Windows;
using Bovender.Mvvm.Messaging;
using Bovender.Extensions;

namespace Bovender.Mvvm.Actions
{
    /// <summary>
    /// Injects a view with a view model that is referenced in a ViewModelMessageContent,
    /// and shows the view non-modally.
    /// </summary>
    public class ShowViewAction : NotificationAction
    {
        #region Public properties

        public string Assembly { get; set; }
        public string View { get; set; }

        #endregion

        #region Overrides

        protected override Window CreateView()
        {
            object obj = Activator.CreateInstance(Assembly, View).Unwrap();
            Window view = obj as Window;
            if (view != null)
            {
                Logger.Info("CreateView: Created {0}", view.GetType().FullName);
                return view;
            }
            else
            {
                Logger.Fatal("Class {0} in assembly {1} is not derived from Window", Assembly, View);
                throw new ArgumentException(String.Format(
                    "Class '{0}' in assembly '{1}' is not derived from Window.",
                    Assembly, View));
            }
        }

        protected override void ShowView(Window view)
        {
            view.ShowInForm();
        }

        protected override ViewModels.ViewModelBase GetDataContext(MessageContent messageContent)
        {
            ViewModelMessageContent vmc = messageContent as ViewModelMessageContent;
            if (vmc != null)
            {
                return vmc.ViewModel;
            }
            else
            {
                Logger.Fatal("GetDataContext: Expected ViewModelMessageContent, got {0}",
                    messageContent == null ? "null" : messageContent.GetType().FullName);
                throw new ArgumentException("Invalid message content: ShowViewAction requires ViewModelMessageContent");
            }
        }

        #endregion

        #region Class logger

        private static NLog.Logger Logger { get { return _logger.Value; } }

        private static readonly Lazy<NLog.Logger> _logger = new Lazy<NLog.Logger>(() => NLog.LogManager.GetCurrentClassLogger());

        #endregion
    }
}
