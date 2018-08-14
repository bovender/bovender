﻿/* ChooseFileOpenAction.cs
 * part of Daniel's XL Toolbox NG
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
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using Bovender;

namespace Bovender.Mvvm.Actions
{
    /// <summary>
    /// Lets the user choose a file name for opening a file.
    /// </summary>
    public class ChooseFileOpenAction : FileDialogActionBase
    {
        protected override FileDialog GetDialog(
            string defaultString,
            string filter)
        {
            Logger.Info("ChooseFileOpenAction.GetDialog");
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = filter;
            dlg.FileName = PathHelpers.GetFileNamePart(defaultString);
            dlg.InitialDirectory = PathHelpers.GetDirectoryPart(defaultString);
            dlg.AddExtension = true;
            dlg.RestoreDirectory = true;
            dlg.SupportMultiDottedExtensions = true;
            dlg.ValidateNames = true;
            dlg.ShowHelp = false;
            return dlg;
        }

        #region Class logger

        private static NLog.Logger Logger { get { return _logger.Value; } }

        private static readonly Lazy<NLog.Logger> _logger = new Lazy<NLog.Logger>(() => NLog.LogManager.GetCurrentClassLogger());

        #endregion
    }
}
