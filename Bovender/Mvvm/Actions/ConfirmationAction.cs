/* ConfirmationAction.cs
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
using Bovender.Mvvm.Views;

namespace Bovender.Mvvm.Actions
{
    /// <summary>
    /// Provides an action to confirm or cancel a message.
    /// Adds a CancelButtonText to the NotificationAction
    /// base class and creates a ConfirmationView rather
    /// than a NotificationView.
    /// </summary>
    public class ConfirmationAction : NotificationAction
    {
        #region Constructor

        public ConfirmationAction()
            : base()
        {
            CancelButtonText = "Cancel";
        }

        #endregion

        #region Overrides

        protected override System.Windows.Window CreateView()
        {
            return new ConfirmationView();
        }

        #endregion
    }
}
