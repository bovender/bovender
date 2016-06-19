/* MessageActionBase.cs
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
using System.Windows;
using System.Windows.Interactivity;
using Bovender.Mvvm.Messaging;
using Bovender.Mvvm.ViewModels;
using Bovender.Extensions;

namespace Bovender.Mvvm.Actions
{
    /// <summary>
    /// Abstract base class for MVVM messaging actions. Derived classes must
    /// implement a CreateView method that returns a view for the view model
    /// that is expected to be received as a message content.
    /// </summary>
    public abstract class MessageActionBase : TriggerAction<FrameworkElement>
    {
        #region Public properties

        public MessageContent Content { get; protected set; }

        #endregion

        #region TriggerAction overrides

        /// <summary>
        /// Invokes the action by creating a view and a corresponding view model.
        /// </summary>
        /// <param name="parameter"><see cref="MessageArgs"/> argument
        /// for the <see cref="Message.Sent"/> event.</param>
        protected override void Invoke(object parameter)
        {
            dynamic args = parameter as MessageArgsBase;
            dynamic content = null;
            dynamic response = null;
            if (args != null)
            {
                content = args.Content;
                response = args.Respond;
            }
            else
            {
                Logger.Warn("Invoke: Parameter is not a MessageArgsBase");
            }
            Invoke(content, response);
        }

        protected void Invoke<T>(T messageContent, Action response)
            where T : MessageContent
        {
            if (messageContent == null)
            {
                Logger.Warn("Invoke: messageContent is null");
            }
            Content = messageContent;
            Window window = CreateView();
            if (window != null)
            {
                MessageActionViewModel vm = CreateViewModel();
                EventHandler responseHandler = null;
                responseHandler = (sender, e) =>
                {
                    vm.RequestCloseView -= responseHandler;
                    if (response != null) response();
                };
                vm.RequestCloseView += responseHandler;
                vm.InjectInto(window);
                ShowView(window);
            }
            else
            {
                Logger.Warn("Invoke: CreateView did not return a Window object!");
            }
        }

        #endregion

        #region Virtual methods

        /// <summary>
        /// Shows the view as a modal dialog. Override this to do something
        /// else with the view.
        /// </summary>
        /// <param name="view">Window object previously created by <see cref="CreateView"/>.</param>
        /// <remarks>This method is called internally by the <see cref="Invoke"/>
        /// method.</remarks>
        protected virtual void ShowView(Window view)
        {
            Logger.Info("ShowView: Showing view as dialog");
            view.ShowDialogInForm();
        }

        /// <summary>
        /// Creates a MessageActionViewModel that wraps this MessageActionBase object.
        /// Can be overridden to create different view models.
        /// </summary>
        /// <returns>MessageActionViewModel wrapping 'this'.</returns>
        protected virtual MessageActionViewModel CreateViewModel()
        {
            Logger.Info("CreateViewModel: Creating default MessageActionViewModel");
            return new MessageActionViewModel(this);
        }

        #endregion

        #region Abstract methods

        /// <summary>
        /// Returns a view that can bind to expected message contents.
        /// </summary>
        /// <returns>Descendant of Window.</returns>
        protected abstract Window CreateView();

        #endregion

        #region Class logger

        protected static NLog.Logger Logger { get { return _logger.Value; } }

        private static readonly Lazy<NLog.Logger> _logger = new Lazy<NLog.Logger>(() => NLog.LogManager.GetCurrentClassLogger());

        #endregion
    }
}
