﻿/* ChooseFileSaveAction.cs
 * part of Daniel's XL Toolbox NG
 * 
 * Copyright 2014-2015 Daniel Kraus
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
using System.IO;
using System.Text.RegularExpressions;
using Bovender;
using Microsoft.Win32;

namespace Bovender.Mvvm.Actions
{
    /// <summary>
    /// Lets the user choose a file name for saving a file.
    /// </summary>
    public class ChooseFileSaveAction : FileDialogActionBase
    {
        protected override dynamic GetDialog(
            string defaultString,
            string filter)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = filter;
            dlg.FileName = PathHelpers.GetFileNamePart(defaultString);
            dlg.InitialDirectory = PathHelpers.GetDirectoryPart(defaultString);
            dlg.AddExtension = true;
            dlg.RestoreDirectory = true;
            dlg.ValidateNames = true;
            dlg.OverwritePrompt = true;
            return dlg;
        }
    }
}
