﻿/* NotificationActionViewModel.cs
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

namespace Bovender.Mvvm.ViewModels
{
    public class NotificationActionViewModel : MessageActionViewModel
    {
        #region Public properties
        
        public string OkButtonText { get { return NotificationAction.OkButtonText; } }

        public string Caption { get { return NotificationAction.Caption; } }

        public string Message { get { return NotificationAction.FormattedText; } }
        
        #endregion

        #region Constructor

        public NotificationActionViewModel(Actions.NotificationAction notificationAction)
            : base(notificationAction)
        { }

        #endregion

        #region Private properties

        private Actions.NotificationAction NotificationAction
        {
            get
            {
                return MessageAction as Actions.NotificationAction;
            }
        }
        
        #endregion
    }
}
