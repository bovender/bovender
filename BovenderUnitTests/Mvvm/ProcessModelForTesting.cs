﻿/* ProcessModelForTesting.cs
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
using Bovender.Mvvm.Models;

namespace Bovender.UnitTests.Mvvm
{
    class ProcessModelForTesting : ProcessModel
    {
        public long Duration { get; private set; }

        public override bool Execute()
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
                if (IsCancellationRequested) break;
            }
            watch.Stop();
            Duration = watch.ElapsedMilliseconds;
            Console.WriteLine(String.Format("Process took {0} ms", Duration));
            return !IsCancellationRequested;
        }
    }
}
