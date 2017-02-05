/* ComHelpers.cs
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
using System.Runtime.InteropServices;
using System.Text;

namespace Bovender
{
    public static class ComHelpers
    {
        public static object ReleaseComObject(object obj)
        {
            if (obj != null && Marshal.IsComObject(obj))
            {
                int count = Marshal.ReleaseComObject(obj);
                Logger.Debug("ReleaseComObject: Ref count after release is {0}", count);
                if (count < 0)
                {
                    string caller = new System.Diagnostics.StackFrame(1).GetMethod().Name;
                    Logger.Warn("ReleaseComObject: Caller: {0}", caller);
                }
                return null;
            }
            else
            {
                Logger.Warn("ReleaseComObject: Obj is null or not a COM object (@ {0})",
                    new System.Diagnostics.StackFrame(1).GetMethod().Name);
                return obj;
            }
        }

        /// <summary>
        /// Tests whether a given object responds to a given method.
        /// </summary>
        /// <param name="obj">Object to query.</param>
        /// <param name="method">Method to test.</param>
        /// <returns>True or false.</returns>
        /// <remarks>
        /// This method cannot be used to determine whether a COM object exposes
        /// a particular method, because System.__COMObject has only a limited number
        /// of methods and does not reveal the COM interfaces methods this way.
        /// </remarks>
        public static bool HasMethod(dynamic obj, string method)
        {
            try
            {
                return obj.GetType().GetMethod(method) != null;
            }
            catch (System.Reflection.AmbiguousMatchException)
            {
                // If there is more than one method with this name, an
                // AmbiguousMatchException is thrown, and we can return true.
                Logger.Debug("HasMethod: Ambiguous match for {0}, returning true", method);
                return true;
            }
        } 

        #region Class logger

        private static NLog.Logger Logger { get { return _logger.Value; } }

        private static readonly Lazy<NLog.Logger> _logger = new Lazy<NLog.Logger>(() => NLog.LogManager.GetCurrentClassLogger());

        #endregion
    }
}
