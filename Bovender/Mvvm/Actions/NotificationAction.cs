/* NotificationAction.cs
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
using System.Windows;
using Bovender.Mvvm.Views;
using Bovender.Mvvm.Messaging;

namespace Bovender.Mvvm.Actions
{
    /// <summary>
    /// Opens a generic WPF dialog window that displays a message and
    /// has a single OK button. The message string can include parameters.
    /// </summary>
    /// <remarks>
    /// The string properties must be declared as dependency properties
    /// so that they can be data-bound in XAML.
    /// </remarks>
    public class NotificationAction : MessageActionBase
    {
        #region Public (dependency) properties

        public string Caption
        {
            get { return (string)GetValue(CaptionProperty); }
            set
            {
                SetValue(CaptionProperty, value);
            }
        }

        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set
            {
                SetValue(MessageProperty, value);
            }
        }

        public string Param1
        {
            get { return (string)GetValue(Param1Property); }
            set
            {
                SetValue(Param1Property, value);
            }
        }

        public string Param2
        {
            get { return (string)GetValue(Param2Property); }
            set
            {
                SetValue(Param2Property, value);
            }
        }

        public string Param3
        {
            get { return (string)GetValue(Param3Property); }
            set
            {
                SetValue(Param3Property, value);
            }
        }

        public string OkButtonText
        {
            get { return (string)GetValue(OkButtonTextProperty); }
            set
            {
                SetValue(OkButtonTextProperty, value);
            }
        }

        public string CancelButtonText
        {
            get { return (string)GetValue(CancelButtonTextProperty); }
            set
            {
                SetValue(CancelButtonTextProperty, value);
            }
        }
    
        /// <summary>
        /// Returns the <see cref="Message"/> string formatted with the
        /// three params.
        /// </summary>
        public string FormattedText
        {
            get
            {
                try
                {
                    if (_formattedText == null)
                    {
                        _formattedText = String.Format(Message, Param1, Param2, Param3);
                    }
                    Logger.Info(_formattedText);
                    return _formattedText;
                }
                catch
                {
                    Logger.Warn("Cannot format notification message: no Message");
                    return "*** No message text given! ***";
                }
            }
        }

        #endregion

        #region Declarations of dependency properties

        public static readonly DependencyProperty CaptionProperty = DependencyProperty.Register(
            "Caption", typeof(string), typeof(NotificationAction));

        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register(
            "Message", typeof(string), typeof(NotificationAction));

        public static readonly DependencyProperty Param1Property = DependencyProperty.Register(
            "Param1", typeof(string), typeof(NotificationAction));

        public static readonly DependencyProperty Param2Property = DependencyProperty.Register(
            "Param2", typeof(string), typeof(NotificationAction));

        public static readonly DependencyProperty Param3Property = DependencyProperty.Register(
            "Param3", typeof(string), typeof(NotificationAction));

        public static readonly DependencyProperty OkButtonTextProperty = DependencyProperty.Register(
            "OkButtonText", typeof(string), typeof(NotificationAction));

        public static readonly DependencyProperty CancelButtonTextProperty = DependencyProperty.Register(
            "CancelButtonText", typeof(string), typeof(NotificationAction));

        #endregion

        #region Constructors

        public NotificationAction()
            : base()
        {
            OkButtonText = "OK";
        }

        public NotificationAction(string caption, string message)
            : this()
        {
            Caption = caption;
            Message = message;
        }

        public NotificationAction(string caption, string message, string okButtonText)
            : this(caption, message)
        {
            OkButtonText = okButtonText;
        }

        public NotificationAction(string caption, string message, string okButtonText, string param)
            : this(caption, message, okButtonText)
        {
            Param1 = param;
        }

        public NotificationAction(string caption, string message, string okButtonText, string param1, string param2)
            : this(caption, message, okButtonText, param1)
        {
            Param2 = param2;
        }

        public NotificationAction(string caption, string message, string okButtonText, string param1, string param2, string param3)
            : this(caption, message, okButtonText, param1, param2)
        {
            Param3 = param3;
        }

        #endregion

        #region Implementation of abstract base methods

        protected override Window CreateView()
        {
            return new NotificationView();
        }

        #endregion

        #region Overrides

        protected override ViewModels.ViewModelBase GetDataContext(MessageContent messageContent)
        {
            if (!String.IsNullOrEmpty(Caption))
            {
                Logger.Info("GetDataContext: Overriding caption");
                messageContent.Caption = Caption;
            }
            if (!String.IsNullOrEmpty(Message))
            {
                Logger.Info("GetDataContext: Overriding message");
                messageContent.Message = FormattedText;
            }
            if (!String.IsNullOrEmpty(OkButtonText))
            {
                Logger.Info("GetDataContext: Overriding OK button text");
                messageContent.OkButtonText = OkButtonText;
            }
            if (!String.IsNullOrEmpty(CancelButtonText))
            {
                Logger.Info("GetDataContext: Overriding OK button text");
                messageContent.CancelButtonText = CancelButtonText;
            }
            return messageContent;
        }

        #endregion

        #region Private fields

        private string _formattedText;

        #endregion

        #region Class logger

        private static NLog.Logger Logger { get { return _logger.Value; } }

        private static readonly Lazy<NLog.Logger> _logger = new Lazy<NLog.Logger>(() => NLog.LogManager.GetCurrentClassLogger());

        #endregion
    }
}
