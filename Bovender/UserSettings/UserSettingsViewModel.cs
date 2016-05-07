/* UserSettingsViewModel.cs
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
using Bovender.Mvvm;
using Bovender.Mvvm.ViewModels;
using Bovender.Mvvm.Messaging;

namespace Bovender.UserSettings
{
    /// <summary>
    /// General view model for UserSettings objects.
    /// </summary>
    public class UserSettingsViewModel : ViewModelBase
    {
        #region Public properties

        public UserSettingsBase UserSettings { get; set; }

        #endregion

        #region Commands

        public DelegatingCommand SaveCommand
        {
            get
            {
                if (_saveCommand == null)
                {
                    _saveCommand = new DelegatingCommand(param => DoSave());
                }
                return _saveCommand;
            }
        }

        #endregion

        #region MVVM messages

        public Message<ViewModelMessageContent> SavedMessage
        {
            get
            {
                if (_savedMessage == null)
                {
                    _savedMessage = new Message<ViewModelMessageContent>();
                }
                return _savedMessage;
            }
        }

        #endregion

        #region Implementation of ViewModelBase

        public override object RevealModelObject()
        {
            return UserSettings;
        }

        #endregion

        #region Constructors

        public UserSettingsViewModel()
            : base()
        { }

        public UserSettingsViewModel(UserSettingsBase userSettings)
            : this()
        {
            UserSettings = userSettings;
        }

        #endregion

        #region Protected methods

        protected virtual void DoSave()
        {
            if (UserSettings != null)
            {
                UserSettings.Save();
                SavedMessage.Send(new ViewModelMessageContent(this));
            }
        }

        #endregion

        #region Private fields

        private DelegatingCommand _saveCommand;
        private Message<ViewModelMessageContent> _savedMessage;

        #endregion
    }
}
