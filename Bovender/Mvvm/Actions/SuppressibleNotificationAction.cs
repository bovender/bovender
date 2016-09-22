/* SuppressibleNotificationAction.cs
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
using System.Windows;

namespace Bovender.Mvvm.Actions
{
    public class SuppressibleNotificationAction : NotificationAction
    {
        #region Properties

        public virtual bool Suppress { get; set; }

        #endregion

        #region DependencyProperties

        public string SuppressMessageText
        {
            get { return (string)GetValue(SuppressMessageTextProperty); }
            set
            {
                SetValue(SuppressMessageTextProperty, value);
            }
        }

        #endregion

        #region Declarations of dependency properties

        public static readonly DependencyProperty SuppressMessageTextProperty = DependencyProperty.Register(
            "SuppressMessageText", typeof(string), typeof(SuppressibleNotificationAction));

        #endregion

        #region Overrides

        protected override void Invoke(object parameter)
        {
            if (!Suppress)
            {
                Logger.Info("Invoke: Notification is not suppressed");
                base.Invoke(parameter);
            }
            else
            {
                Logger.Info("Invoke: Notification is suppressed");
            }
        }

        protected override System.Windows.Window CreateView()
        {
            return new Bovender.Mvvm.Views.SuppressibleNotificationView();
        }

        #endregion

        #region Class logger

        private static NLog.Logger Logger { get { return _logger.Value; } }

        private static readonly Lazy<NLog.Logger> _logger = new Lazy<NLog.Logger>(() => NLog.LogManager.GetCurrentClassLogger());

        #endregion
    }
}
