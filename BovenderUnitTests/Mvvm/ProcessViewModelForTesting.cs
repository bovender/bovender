/* ProcessViewModelForTesting.cs
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
using System.Threading;
using System.Threading.Tasks;
using Bovender.Mvvm.ViewModels;

namespace Bovender.UnitTests.Mvvm
{
    class ProcessViewModelForTesting : ProcessViewModelBase
    {
        public long Duration { get; private set; }

        public void Start()
        {
            StartProcess();
        }

        protected override void Execute()
        {
            _processing = true;
            Task.Factory.StartNew((Action)(() =>
                {
                    try
                    {
                        var watch = System.Diagnostics.Stopwatch.StartNew();
                        for (int i = 0; i < 40000; i++)
                        {
                            int n = i;
                            int f = n;
                            for (int j = 0; j < n; j++)
                            {
                                f *= j;
                            }
                        }
                        watch.Stop();
                        Duration = watch.ElapsedMilliseconds;
                        Console.WriteLine(String.Format("Process took {0} ms", Duration));
                        _processing = false;
                        SendCompletionMessage();
                    }
                    catch (Exception e)
                    {
                        SendProcessFailedMessage(e);
                    }
                }));
        }

        protected override void CancelProcess()
        {
            _cancelled = true;
        }

        protected override int GetPercentCompleted()
        {
            return 33;
        }

        protected override bool IsProcessing()
        {
            return _processing;
        }

        public override object RevealModelObject()
        {
            throw new NotImplementedException();
        }

        private bool _cancelled;
        private bool _processing;
    }
}
