/* StringExtensions.cs
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

namespace Bovender.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Truncates a string and adds three dots (ellipsis) so that the resulting
        /// string is at most "<paramref name="length"/>" characters long.
        /// </summary>
        /// <param name="s">String to truncate</param>
        /// <param name="length">Maximum length of resulting string, including
        /// ellipsis; an exception is thrown if this is 5 or less.</param>
        /// <returns>String at most "<paramref name="length"/>" characters long.</returns>
        public static string TruncateWithEllipsis(this string s, int length)
        {
            if (length <= 5)
            {
                throw new ArgumentException("Cannot truncate string with ellipsis to 5 or less characters.");
            }
            return s.Length <= length ? s : s.Substring(0, length - 3) + "...";
        }
    }
}
