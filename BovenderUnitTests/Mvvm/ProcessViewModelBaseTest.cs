/* ProcessViewModelBaseTest.cs
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
using NUnit.Framework;
using Bovender.Mvvm.ViewModels;
using System.Threading;

namespace Bovender.UnitTests.Mvvm
{
    [TestFixture]
    class ProcessViewModelBaseTest
    {
        [SetUp]
        public void Setup()
        {
            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
            Logging.LogFile.Default.EnableDebugLogging();
            _model = new ProcessModelForTesting();
            _viewModel = new ProcessViewModelForTesting(_model);
            _messageContent = null;
        }

        [Test]
        public void ShowProgress()
        {
            bool showProgressWasSent = false;
            bool completed = false;
            _viewModel.ProcessFinishedMessage.Sent += (s, a) =>
            {
                Logger.Info("ProcessFinishedMessage was sent");
                completed = true;
            };
            _viewModel.ShowProgressMessage.Sent += (sender, args) =>
            {
                Logger.Info("ShowProgressMessage was sent");
                _messageContent = args.Content;
                showProgressWasSent = true;
            };
            bool abort = false;
            Timer t = new Timer((obj) =>
                {
                    if (_messageContent != null && _messageContent.Processing)
                    {
                        Logger.Info("aborting...");
                        abort = true;
                    }
                }, null, 5000, Timeout.Infinite);
            _viewModel.StartProcess();
            while (!completed && !abort) ;
            t.Dispose();
            if (abort)
            {
                Logger.Info("Cancelling...");
                _viewModel.CancelProcess();
            }
            Logger.Info("Asserting...");
            Assert.IsFalse(abort, "Task was aborted");
            Assert.IsTrue(_model.Duration > 1500, "Process took less than 1.5 seconds - increase faculty loop?");
            Assert.IsTrue(showProgressWasSent, "ShowProgress message should have been sent");
            Assert.IsTrue(_messageContent.WasSuccessful, "WasSuccessful should be true");
            Assert.IsNull(_messageContent.Exception, "Exception should be null");
        }

        [Test]
        public void ProcessException()
        {
            bool showProgressWasSent = false;
            bool completed = false;
            _model = new ExceptionProcessModelForTesting();
            _viewModel = new ProcessViewModelForTesting(_model);
            _viewModel.ProcessFinishedMessage.Sent += (s, a) =>
            {
                Logger.Info("ProcessFinishedMessage was sent");
                completed = true;
            };
            _viewModel.ShowProgressMessage.Sent += (sender, args) =>
            {
                Logger.Info("ShowProgressMessage was sent");
                _messageContent = args.Content;
                showProgressWasSent = true;
            };
            bool abort = false;
            Timer t = new Timer((obj) =>
            {
                if (_messageContent.Processing)
                {
                    Logger.Info("aborting...");
                    abort = true;
                }
            }, null, 5000, Timeout.Infinite);
            _viewModel.StartProcess();
            while (!completed && !abort) ;
            t.Dispose();
            if (abort)
            {
                Logger.Info("Cancelling...");
                _viewModel.CancelProcess();
            }
            Logger.Info("Asserting...");
            Assert.IsFalse(abort, "Task was aborted");
            Assert.IsTrue(_model.Duration > 1500, "Process took less than 1.5 seconds - increase faculty loop?");
            Assert.IsTrue(showProgressWasSent, "ShowProgress message should have been sent");
            Assert.IsFalse(_messageContent.WasSuccessful, "WasSuccessful should be false");
            Assert.IsInstanceOf<ExceptionForTestingPurposes>(_messageContent.Exception, "Exception was not handed over.");
        }

        [Test]
        public void CancelProcessViaViewModel()
        {
            bool completed = false;
            _viewModel.ProcessFinishedMessage.Sent += (s, a) =>
            {
                Logger.Info("ProcessFinishedMessage was sent");
                completed = true;
            };
            _viewModel.ShowProgressMessage.Sent += (sender, args) =>
            {
                Logger.Info("ShowProgressMessage was sent");
                _messageContent = args.Content;
            };
            bool abort = false;
            Timer t = new Timer((obj) =>
                {
                    if (_messageContent.Processing)
                    {
                        Logger.Info("aborting...");
                        abort = true;
                    }
                }, null, 5000, Timeout.Infinite);
            Timer abortTimer = new Timer((obj) =>
                {
                    Logger.Info("Now simulating abort...");
                    _viewModel.CancelProcess();
                }, null, 1000, Timeout.Infinite);
            _viewModel.StartProcess();
            while (!completed && !abort) ;
            t.Dispose();
            if (abort)
            {
                Logger.Info("Cancelling...");
                _viewModel.CancelProcess();
            }
            abortTimer.Dispose();
            Logger.Info("Asserting...");
            Assert.IsFalse(abort, "Task was aborted");
            Assert.IsTrue(_model.Duration < 1300, "Process took more than 1.3 seconds - was not aborted?");
            Assert.IsTrue(_messageContent.WasCancelled, "WasCancelled should be true");
            Assert.IsNull(_messageContent.Exception, "Exception should be null");
        }

        [Test]
        public void CancelProcessViaMessageContent()
        {
            bool completed = false;
            _viewModel.ProcessFinishedMessage.Sent += (s, a) =>
            {
                Logger.Info("ProcessFinishedMessage was sent");
                completed = true;
            };
            _viewModel.ShowProgressMessage.Sent += (sender, args) =>
            {
                Logger.Info("ShowProgressMessage was sent");
                _messageContent = args.Content;
            };
            bool abort = false;
            Timer t = new Timer((obj) =>
            {
                if (_messageContent != null && _messageContent.Processing)
                {
                    Logger.Info("aborting...");
                    abort = true;
                }
            }, null, 5000, 2000);
            Timer abortTimer = new Timer((obj) =>
            {
                if (_messageContent != null)
                {
                    Logger.Info("Now simulating cancellation...");
                    _messageContent.CancelCommand.Execute(null);
                }
            }, null, 1200, 100);
            _viewModel.StartProcess();
            while (!completed && !abort) ;
            t.Dispose();
            if (abort)
            {
                Logger.Info("Cancelling...");
                _viewModel.CancelProcess();
            }
            abortTimer.Dispose();
            Logger.Info("Asserting...");
            Assert.IsFalse(abort, "Task was aborted");
            Assert.IsTrue(_model.Duration < 1500, "Process took more than 1.5 seconds - was not aborted?");
            Assert.IsTrue(_messageContent.WasCancelled, "WasCancelled should be true");
            Assert.IsNull(_messageContent.Exception, "Exception should be null");
        }

        #region Private fields

        ProcessModelForTesting _model;
        ProcessViewModelForTesting _viewModel;
        Bovender.Mvvm.Messaging.ProcessMessageContent _messageContent;

        #endregion

        #region Class logger

        private static NLog.Logger Logger { get { return _logger.Value; } }

        private static readonly Lazy<NLog.Logger> _logger = new Lazy<NLog.Logger>(() => NLog.LogManager.GetCurrentClassLogger());

        #endregion
    }
}
