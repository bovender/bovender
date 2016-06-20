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

        public MessageContent Content { get; set; }

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
            Content = messageContent;
            ViewModelBase viewModel = GetDataContext(messageContent);
            Window view = CreateView();
            if (view != null)
            {
                if (viewModel != null)
                {
                    EventHandler responseHandler = null;
                    responseHandler = (sender, e) =>
                    {
                        viewModel.RequestCloseView -= responseHandler;
                        if (response != null) response();
                    };
                    viewModel.RequestCloseView += responseHandler;
                    if (view.DataContext == null)
                    {
                        viewModel.InjectInto(view);
                    }
                    else
                    {
                        Logger.Fatal("Invoke: Got view from CreateView() that already has a data context!");
                        Logger.Fatal("Invoke: Data context must be returned from GetDataContext() only!");
                        throw new InvalidOperationException("CreateView() must not create a view with a data context");
                    }
                }
                else
                {
                    Logger.Info("Invoke: Did not get view model, cannot data bind view or set up event handlers");
                }
                if (view.DataContext != null)
                {
                    Logger.Info("Invoke: View is data-bound to {0}", view.DataContext.GetType().FullName);
                }
                else
                {
                    Logger.Info("Invoke: View has no data context");
                }
                ShowView(view);
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
        /// Gets a ViewModelBase object that will be injected into the view
        /// In the abstract base class, this returns
        /// the <paramref name="messageContent"/> parameter as-is.
        /// </summary>
        /// <param name="messageContent">Message content that will be data-bound
        /// to the view.</param>
        /// <returns>ViewModelBase object</returns>
        protected virtual ViewModelBase GetDataContext(MessageContent messageContent)
        {
            return messageContent;
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

        private static NLog.Logger Logger { get { return _logger.Value; } }

        private static readonly Lazy<NLog.Logger> _logger = new Lazy<NLog.Logger>(() => NLog.LogManager.GetCurrentClassLogger());

        #endregion
    }
}
