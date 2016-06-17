/* ProcessViewModelBaseTest.cs
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
using NUnit.Framework;
using Bovender.Mvvm.ViewModels;

namespace Bovender.UnitTests.Mvvm
{
    [TestFixture]
    class ProcessViewModelBaseTest
    {
        [Test]
        public void ShowProgress()
        {
            ProcessViewModelForTesting vm = new ProcessViewModelForTesting();
            bool showProgressWasSent = false;
            bool failure = false;
            bool completed = false;
            vm.ShowProgressMessage.Sent += (sender, args) =>
            {
                showProgressWasSent = true;
                args.Content.CompletedMessage.Sent += (s, a) =>
                {
                    completed = true;
                };
            };
            vm.ProcessFailedMessage.Sent += (sender2, args2) =>
            {
                failure = true;
            };
            vm.Start();
            while (!completed) ;
            Assert.IsTrue(vm.Duration > 1500, "Process took less than 1.5 seconds - increase faculty loop?");
            Assert.IsTrue(showProgressWasSent, "ShowProgress message should have been sent");
            Assert.IsFalse(failure, "ProcessFailedMessage should NOT have been sent");
        }
    }
}
