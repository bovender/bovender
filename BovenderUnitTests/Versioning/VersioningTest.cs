﻿/* VersioningTest.cs
 * part of Bovender
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
using System.Reflection;
using Bovender.Versioning;
using NUnit.Framework;

namespace XLToolbox.Test
{
    [TestFixture]
    public class VersioningTest
    {
        [Test]
        [TestCase("0.1.2", 0, 1, 2, "", "")]
        [TestCase("10.20.30-0.0.1", 10, 20, 30, "0.0.1", "")]
        [TestCase("0.1.2-alpha.1", 0, 1, 2, "alpha.1", "")]
        [TestCase("0.1.2-0.0.1+githash", 0, 1, 2, "0.0.1", "githash")]
        [TestCase("0.1.2+githash", 0, 1, 2, "", "githash")]
        public void ParseSemanticVersion(string version, int major, int minor, int patch,
            string preRelease, string build)
        {
            SemanticVersion semVer = new SemanticVersion(version);
            Assert.AreEqual(major, semVer.Major, "Major version does not match");
            Assert.AreEqual(minor, semVer.Minor, "Minor version does not match");
            Assert.AreEqual(patch, semVer.Patch, "Patch number does not match");
            Assert.AreEqual(build, semVer.Build, "Build information does not match");
        }

        // This used to work when the test was part of Daniel's XL Toolbox
        // [Test]
        // public void GetCurrentVersion()
        // {
        //     Assembly assembly = Assembly.GetExecutingAssembly();
        //     SemanticVersion v = SemanticVersion.CurrentVersion(assembly);
        //     Assert.AreEqual(7, v.Major);
        // }

        [Test]
        [ExpectedException(typeof(InvalidVersionStringException))]
        public void InvalidVersionThrowsError()
        {
            SemanticVersion v = new SemanticVersion("2.0");
        }

        [Test]
        [TestCase("1.0.0", "1.0.1")]
        [TestCase("1.0.0", "1.1.0")]
        [TestCase("1.0.1", "2.0.0")]
        [TestCase("1.1.1", "2.0.0")]
        [TestCase("1.1.1-0.1.2", "1.1.1")]
        [TestCase("1.1.1-alpha.10", "1.1.1")]
        [TestCase("1.1.1-beta.2", "1.1.1")]
        [TestCase("1.1.1-alpha.2", "1.1.1-beta.1")]
        [TestCase("1.1.1-rc.1", "1.1.1-rc.2")]
        public void CompareVersions(string lower, string higher)
        {
            SemanticVersion lowerVersion = new SemanticVersion(lower);
            SemanticVersion higherVersion = new SemanticVersion(higher);
            Assert.Greater(higherVersion, lowerVersion);
            Assert.True(lowerVersion < higherVersion);
            Assert.True(higherVersion > lowerVersion);
        }

        [Test]
        [TestCase("1.0.0")]
        [TestCase("1.0.0-3.2.1")]
        [TestCase("1.0.0-alpha.3")]
        [TestCase("1.0.0-rc.2")]
        [TestCase("1.0.0+20160507")]
        [TestCase("1.0.0-alpha.3+20160507")]
        public void Idempotence(string versionString)
        {
            SemanticVersion v = new SemanticVersion(versionString);
            Assert.AreEqual(versionString, v.ToString());
        }
    }
}
