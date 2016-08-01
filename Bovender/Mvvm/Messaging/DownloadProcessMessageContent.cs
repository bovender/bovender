using Bovender.Mvvm.ViewModels;
/* DownloadProcessMessageContent.cs
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

namespace Bovender.Mvvm.Messaging
{
    public class DownloadProcessMessageContent : ProcessMessageContent
    {
        #region Public properties

        public double DownloadMegaBytesReceived
        {
            get
            {
                return _received;
            }
            set
            {
                _received = value;
                OnPropertyChanged("DownloadMegaBytesReceived");
            }
        }

        public double DownloadMegaBytesTotal
        {
            get
            {
                return _total;
            }
            set
            {
                _total = value;
                OnPropertyChanged("DownloadMegaBytesTotal");
            }
        }
        
        #endregion

        #region Constructors

        public DownloadProcessMessageContent()
            : base()
        { }

        public DownloadProcessMessageContent(ViewModelBase viewModel)
            : base(viewModel)
        { }

        public DownloadProcessMessageContent(Action cancelProcess)
            : base(cancelProcess)
        { }

        public DownloadProcessMessageContent(ViewModelBase viewModel, Action cancelProcess)
            : base(viewModel, cancelProcess)
        { }    

        #endregion

        #region Private fields

        private double _received;
        private double _total;
        
        #endregion
    }
}
