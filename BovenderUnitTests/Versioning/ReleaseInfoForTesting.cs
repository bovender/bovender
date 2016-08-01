/* ReleaseInfoForTesting.cs
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
using Bovender.Versioning;

namespace Bovender.UnitTests.Versioning
{
    class ReleaseInfoForTesting : ReleaseInfo
    {
        public override bool Fetch()
        {
            return true; // do not attempt to fetch anything from the internet
        }
    
        /// <summary>
        /// Creates a new ReleaseInfo object with the given raw release information.
        /// This constructor exists to facilitate testing.
        /// </summary>
        /// <param name="rawReleaseInfo"></param>
        internal ReleaseInfoForTesting(string rawReleaseInfo)
            : base()
        {
            RawReleaseInfo = rawReleaseInfo;
            Parse(rawReleaseInfo);
        }
}
}
