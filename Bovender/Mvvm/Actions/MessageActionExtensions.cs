using System;
/* MessageActionExtensions.cs
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
using System.Windows.Interactivity;
using Bovender.Mvvm.Messaging;

namespace Bovender.Mvvm.Actions
{
    public static class MessageActionExtensions
    {
        /// <summary>
        /// Invokes a <see cref="TriggerAction"/> with the specified parameter.
        /// </summary>
        /// <param name="messageArgs">A MessageArgs&lt;T&gt; object.</param>
        /// <remarks>
        /// <para>
        /// After http://stackoverflow.com/a/12977944/270712.
        /// </para>
        /// <para>
        /// The <paramref name="messageArgs" /> parameter should be a MessageArgs&lt;T&gt;
        /// object. However, because a generic class cannot be passed as a parameter without
        /// specifying the type parameter, the parameter class EventArgs is used, which is
        /// the direct ancestor of MessageArgs&lt;T&gt;.
        /// </para>
        /// </remarks>
        public static void Invoke(this MessageActionBase action, EventArgs messageArgs)
        {
            NonUiTrigger trigger = new NonUiTrigger();
            trigger.Actions.Add(action);

            try
            {
                trigger.Invoke(messageArgs);
            }
            finally
            {
                trigger.Actions.Remove(action);
            }
        }

        public static void InvokeWithContent<T>(this MessageActionBase action, T messageContent)
            where T: MessageContent
        {
            NonUiTrigger trigger = new NonUiTrigger();
            trigger.Actions.Add(action);

            try
            {
                trigger.Invoke(new MessageArgs<T>(messageContent, null));
            }
            finally
            {
                trigger.Actions.Remove(action);
            }
        }


        /// <summary>
        /// Invokes a <see cref="TriggerAction"/> and sets the Content property to
        /// a newly created Messaging.MessageArgs object .
        /// </summary>
        public static void Invoke(this MessageActionBase action)
        {
            action.Invoke(new MessageArgs<MessageContent>(new MessageContent(), null));
        }
    }
}