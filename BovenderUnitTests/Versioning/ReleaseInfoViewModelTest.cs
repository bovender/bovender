/* ReleaseInfoViewModelTest.cs
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
using NUnit.Framework;
using Bovender.Versioning;
using System.Threading;

namespace Bovender.UnitTests.Versioning
{
    [TestFixture]
    public class ReleaseInfoViewModelTest
    {
        [SetUp]
        public void Setup()
        {
            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
            Logging.LogFile.Default.EnableDebugLogging();
        }
            
        [Test]
        public void FetchReleaseInfo()
        {
            string raw = "2.0.0\r\nhttp://release.exe\r\nabcdef1234567890\r\nSummary";
            ReleaseInfoForTesting ri = new ReleaseInfoForTesting(raw);
            ReleaseInfoViewModel rvm = new ReleaseInfoViewModel(ri, new SemanticVersion("1.0.0"));
            bool updateAvailableMessageSent = false;
            bool updateNotAvailableMessageSent = false;
            bool failureMessageSent = false;
            bool busy = true;
            rvm.UpdateAvailableMessage.Sent += (sender, args) =>
            {
                updateAvailableMessageSent = true;
            };
            rvm.NoUpdateAvailableMessage.Sent += (sender, args) =>
            {
                updateNotAvailableMessageSent = true;
            };
            rvm.ExceptionMessage.Sent += (sender, args) =>
            {
                failureMessageSent = true;
            };
            rvm.ProcessFinishedMessage.Sent += (sender, args) =>
            {
                busy = false;
            };
            rvm.CheckForUpdateCommand.Execute(null);
            while (busy) ;
            Assert.IsTrue(updateAvailableMessageSent, "UpdateAvailableMessage was not sent");
            Assert.IsFalse(updateNotAvailableMessageSent, "NoUpdateAvailableMessage was sent?!");
            Assert.IsFalse(failureMessageSent, "FailureMessage");
        }

        [Test]
        public void FetchReleaseInfoNotNewer()
        {
            string raw = "2.0.0\r\nhttp://release.exe\r\nabcdef1234567890\r\nSummary";
            ReleaseInfoForTesting ri = new ReleaseInfoForTesting(raw);
            ReleaseInfoViewModel rvm = new ReleaseInfoViewModel(ri, new SemanticVersion("2.0.0"));
            bool updateAvailableMessageSent = false;
            bool updateNotAvailableMessageSent = false;
            bool failureMessageSent = false;
            bool busy = true;
            rvm.UpdateAvailableMessage.Sent += (sender, args) =>
            {
                updateAvailableMessageSent = true;
            };
            rvm.NoUpdateAvailableMessage.Sent += (sender, args) =>
            {
                updateNotAvailableMessageSent = true;
            };
            rvm.ExceptionMessage.Sent += (sender, args) =>
            {
                failureMessageSent = true;
            };
            rvm.ProcessFinishedMessage.Sent += (sender, args) =>
            {
                busy = false;
            };
            rvm.CheckForUpdateCommand.Execute(null);
            while (busy) ;
            Assert.IsFalse(updateAvailableMessageSent);
            Assert.IsTrue(updateNotAvailableMessageSent);
            Assert.IsFalse(failureMessageSent, "FailureMessage");
        }

        [Test]
        public void FetchInvalidReleaseInfo()
        {
            string raw = "2.0.0";
            ReleaseInfoForTesting ri = new ReleaseInfoForTesting(raw);
            ReleaseInfoViewModel rvm = new ReleaseInfoViewModel(ri, new SemanticVersion("1.0.0"));
            bool updateAvailableMessageSent = false;
            bool updateNotAvailableMessageSent = false;
            bool failureMessageSent = false;
            bool busy = true;
            rvm.UpdateAvailableMessage.Sent += (sender, args) =>
            {
                updateAvailableMessageSent = true;
            };
            rvm.NoUpdateAvailableMessage.Sent += (sender, args) =>
            {
                updateNotAvailableMessageSent = true;
            };
            rvm.ExceptionMessage.Sent += (sender, args) =>
            {
                failureMessageSent = true;
            };
            rvm.ProcessFinishedMessage.Sent += (sender, args) =>
            {
                busy = false;
            };
            rvm.CheckForUpdateCommand.Execute(null);
            while (busy) ;
            Assert.IsFalse(updateAvailableMessageSent, "UpdateAvailableMessage");
            Assert.IsFalse(updateNotAvailableMessageSent, "NoUpdateAvailableMessage");
            Assert.IsTrue(failureMessageSent, "FailureMessage");
        }
    }
}
