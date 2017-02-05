/* ProcessModel.cs
 * part of Bovender framework
 * 
 * Copyright 2014-2017 Daniel Kraus
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
    /// Abstract base class for models that perform a lengthy process asynchronously.
    /// This class is indended to be used with
    /// Bovender.Mvvm.ViewModels.ProcessViewModelBase.
    /// </summary>
    public abstract class ProcessModel : IProcessModel
    {
        #region Public methods

        /// <summary>
        /// Cancels the current process
        /// </summary>
        public void Cancel()
        {
            IsCancellationRequested = true;
            OnCancelling();
        }
        
        #endregion

        #region Events

        public event EventHandler<ProcessModelEventArgs> Cancelling;

        #endregion

        #region Abstract methods

        /// <summary>
        /// This method may be called by a ProcessViewModelBase-derived class
        /// in a worker task that wraps this method in a try...catch structure.
        /// The implementation does not need to and should not handle tasks
        /// or threads itself.
        /// </summary>
        /// <returns>True if successful, false if failed or cancelled.</returns>
        public abstract bool Execute();

        #endregion

        #region Protected methods

        protected virtual void OnCancelling()
        {
            EventHandler<ProcessModelEventArgs> h = Cancelling;
            if (h != null)
            {
                Logger.Info("Raising Cancel event; {0} subscriber(s)", h.GetInvocationList().Length);
                h(this, new ProcessModelEventArgs(this));
            }
        }

        #endregion

        #region Protected properties

        protected ProcessModel Dependent { get; set; }

        /// <summary>
        /// This property is set to true by the Cancel() method.
        /// The implementation of the Execute method should query
        /// this property during the process.
        /// </summary>
        protected bool IsCancellationRequested { get; set; }

        #endregion

        #region Class logger

        private static NLog.Logger Logger { get { return _logger.Value; } }

        private static readonly Lazy<NLog.Logger> _logger = new Lazy<NLog.Logger>(() => NLog.LogManager.GetCurrentClassLogger());

        #endregion
    }
}
