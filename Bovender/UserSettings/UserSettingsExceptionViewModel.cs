/* UserSettingsExceptionViewModel.cs
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

namespace Bovender.UserSettings
{
    /// <summary>
    /// General view model for user settings-related exceptions.
    /// </summary>
    public class UserSettingsExceptionViewModel : ViewModelBase
    {
        #region Public properties

        public UserSettingsBase UserSettings { get; set; }

        public Exception Exception
        {
            get
            {
                if (UserSettings != null)
                {
                    return UserSettings.Exception;
                }
                else
                {
                    return null;
                }
            }
        }

        public string Message
        {
            get
            {
                Exception e = Exception;
                if (e != null)
                {
                    return e.Message;
                }
                else
                {
                    return String.Empty;
                }
            }
        }

        public string InnerMessage
        {
            get
            {
                Exception e = Exception;
                if (e != null && e.InnerException != null)
                {
                    return e.InnerException.Message;
                }
                else
                {
                    return String.Empty;
                }
            }
        }

        #endregion

        #region Constructors

        public UserSettingsExceptionViewModel()
            : base()
        { }

        public UserSettingsExceptionViewModel(UserSettingsBase userSettings)
            : this()
        {
            UserSettings = userSettings;
        }

        #endregion

        #region Commands
        
        #endregion

        #region Implementation of ViewModelBase

        /// <summary>
        /// Returns the exception, if any.
        /// </summary>
        public override object RevealModelObject()
        {
            return Exception;
        }

        #endregion

        #region Private fields

        #endregion
    }
}
