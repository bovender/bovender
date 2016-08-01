/* IReleaseInfo.cs
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

namespace Bovender.Versioning
{
    /// <summary>
    /// Interface for classes that fetch current release information.
    /// It is expected that fetching the information (from whatever
    /// source) is performed synchronously.
    /// </summary>
    public interface IReleaseInfo
    {
        ReleaseInfoStatus Status { get; }

        /// <summary>
        /// Gets or sets the version of the release.
        /// </summary>
        SemanticVersion ReleaseVersion { get; }

        /// <summary>
        /// Gets or sets the release summary.
        /// </summary>
        string Summary { get; }

        /// <summary>
        /// Gets or sets the URI for the release download.
        /// </summary>
        Uri DownloadUri { get; }

        /// <summary>
        /// Gets or sets the expected checksum hash for the
        /// binary file. Depending on the length of this string,
        /// the hash may be a SHA-1 or SHA-256 or other checksum.
        /// </summary>
        string ExpectedHash { get; }

        /// <summary>
        /// Gets or sets the raw, unparsed release information.
        /// </summary>
        string RawReleaseInfo { get; }

        Exception Exception { get; }

        /// <summary>
        /// Updates the release information.
        /// </summary>
        /// <returns>
        /// True if successful, false if not.
        /// </returns>
        bool Fetch();
    }
}
