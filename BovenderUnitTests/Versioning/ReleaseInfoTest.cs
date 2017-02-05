/* ReleaseInfoTest.cs
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
using System.IO;
using NUnit.Framework;
using Bovender.Versioning;

namespace Bovender.UnitTests.Versioning
{
    [TestFixture]
    class ReleaseInfoTest
    {
        [Test]
        public void ParseReleaseInfo()
        {
            string version = "1.2.3";
            string url = "http://example.com/$VERSION/release-$VERSION.exe";
            string hash = "db9ff4b6213b52020e8b4b818f4705773d27717cad64d5587213b31597b7df89 Dummy SHA-256 hash";
            string summary = "This\r\nis a multi-line\r\nrelease summary.";
            string raw = version + "\r\n" + url + "\r\n" + hash + "\r\n" + summary + "\r\n";
            ReleaseInfoForTesting ri = new ReleaseInfoForTesting(raw);
            Assert.AreEqual(raw, ri.RawReleaseInfo, "Raw release info");
            Assert.AreEqual(version, ri.ReleaseVersion.ToString(), "ReleaseVersion");
            Assert.AreEqual(String.Format("http://example.com/{0}/release-{0}.exe", version), ri.DownloadUri.ToString(),
                "Version-substituted URL");
            Assert.AreEqual(summary.Replace("\r\n", " "), ri.Summary, "Summary");
        }

        [Test]
        public void ParsePartialReleaseInfo()
        {
            string version = "1.2.3";
            string url = "http://example.com/$VERSION/release-$VERSION.exe";
            string raw = version + "\r\n" + url + "\r\n";
            ReleaseInfoForTesting ri = new ReleaseInfoForTesting(raw);
            Assert.AreEqual(ReleaseInfoStatus.FailureToParse, ri.Status, "Status should indicate failure to parse");
            Assert.AreEqual(raw, ri.RawReleaseInfo, "Raw release info");
            Assert.AreEqual(null, ri.ReleaseVersion, "ReleaseVersion should be null");
        }
    }
}
