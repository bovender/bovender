/* NotificationViewModel.cs
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
using Bovender.Mvvm.ViewModels;
using Bovender.Mvvm.Actions;
using Bovender.Mvvm.Messaging;

namespace Bovender.Mvvm.ViewModels
{
    /// <summary>
    /// Base class for MessageActions
    /// </summary>
    public class MessageActionViewModel : ViewModelBase
    {
        #region Properties

        public MessageContent Content { get { return MessageAction.Content; } }
        
        #endregion

        #region Constructor

        public MessageActionViewModel(MessageActionBase messageAction)
            : base()
        {
            if (messageAction == null)
            {
                throw new ArgumentNullException("MessageActionViewModel requires MessageAction object; null given.");
            }
            MessageAction = messageAction;
        }

        #endregion

        #region Implementation of ViewModelBase

        public override object RevealModelObject()
        {
            return MessageAction;
        }
        
        #endregion

        #region Protected properties

        protected MessageActionBase MessageAction { get; private set; }

        #endregion
    }
}
