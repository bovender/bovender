/* ProcessModel.cs
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

namespace Bovender.Mvvm.Models
{
    /// <summary>
    /// Base class for models that perform a lengthy process asynchronously.
    /// This class provides events to signal process termination and success
    /// or failure. It is indended to be used with
    /// Bovender.Mvvm.ViewModels.ProcessViewModelBase.
    /// </summary>
    public class ProcessModel
    {
        #region Events

        public event EventHandler<ProcessModelEventArgs> ProcessFailed;

        public event EventHandler<ProcessModelEventArgs> ProcessSucceeded;

        #endregion

        #region Event raising methods

        protected virtual void OnProcessSucceeded()
        {
            Logger.Info("Process succeeded");
            EventHandler<ProcessModelEventArgs> handler = ProcessSucceeded;
            if (handler != null)
            {
                Logger.Info("Raising event for {0} subscribers", handler.GetInvocationList().Length);
                handler(this, new ProcessModelEventArgs(this));
            }
        }

        protected virtual void OnProcessFailed(Exception e)
        {
            Logger.Warn("Process failed");
            Logger.Warn(e);
            EventHandler<ProcessModelEventArgs> handler = ProcessFailed;
            if (handler != null)
            {
                Logger.Info("Raising event for {0} subscribers", handler.GetInvocationList().Length);
                handler(this, new ProcessModelEventArgs(this, e));
            }
        }

        #endregion

        #region Class logger

        protected static NLog.Logger Logger { get { return _logger.Value; } }

        private static readonly Lazy<NLog.Logger> _logger = new Lazy<NLog.Logger>(() => NLog.LogManager.GetCurrentClassLogger());

        #endregion
    }
}
