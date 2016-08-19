/* ObjectExtensions.cs
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

namespace Bovender.Extensions
{
    public static class ObjectExtensions
    {
        public static string ComputeMD5Hash(this object obj)
        {
            return CommonHelpers.ComputeMD5Hash(obj);
        }

        /// <summary>
        /// If the object is not null and has an underlying runtime-callable wrapper (RCW) for
        /// a COM object, Marshal.ReleaseComObject is called with the object.
        /// </summary>
        /// <param name="obj">Object whose COM association to release. This must not be a
        /// dynamic type!</param>
        /// <returns>Null if the COM object was released or if the object was null, or the object 
        /// itself if it does not have an underlying COM object.</returns>
        public static object ReleaseComObject(this object obj)
        {
            if (obj != null && Marshal.IsComObject(obj))
            {
                Marshal.ReleaseComObject(obj);
                return null;
            }
            else
            {
                return obj;
            }
        }

        public static dynamic ReleaseDynamicComObject(dynamic obj)
        {
            if (obj != null && Marshal.IsComObject(obj))
            {
                Marshal.ReleaseComObject(obj);
                return null;
            }
            else
            {
                return obj;
            }
        }
    }
}
