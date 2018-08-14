/* Array.cs
 * part of Bovender framework
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bovender.Extensions
{
    /// <summary>
    /// Extensions methods for arrays.
    /// </summary>
    public static class ArrayExtensions
    {
        /// <summary>
        /// Returns a subarray from an array.
        /// </summary>
        /// <typeparam name="T">Type of the array elements.</typeparam>
        /// <param name="array">Source array</param>
        /// <param name="start">Start index of the slice</param>
        /// <param name="length">Length of the slice</param>
        /// <returns>Subarray of type <typeparamref name="T"/> containing
        /// <paramref name="length"/> elements from <paramref name="array"/>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="length"/>
        /// is 0 or less, if start is lower than 0, or if start is larger than the
        /// largest index in <paramref name="array"/>.</exception>
        public static T[] Slice<T>(this T[] array, int start, int length)
        {
            if (length <= 0 || length > array.Length - start)
            {
                throw new ArgumentOutOfRangeException(
                    "length",
                    String.Format(
                        "Length of sliced array must be between 1 and {0}.",
                        array.Length - start
                    )
                );
            }

            if (start < 0 || start > array.Length - 1)
            {
                throw new ArgumentOutOfRangeException(
                    "start",
                    String.Format(
                        "Start index of sliced array is {0}; array has {1} elements.",
                        start, array.Length
                    )
                );
            }

            T[] slice = new T[length];
            Array.Copy(array, start, slice, 0, length);
            return slice;
        }
    }
}
