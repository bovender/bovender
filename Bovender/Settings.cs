﻿/* Settings.cs
 * part of Daniel's XL Toolbox NG
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

namespace Bovender
{
    /// <summary>
    /// Provides easy public access to this assembly's settings file,
    /// e.g. the user's e-mail address used for submitting exception
    /// reports.
    /// </summary>
    /// <remarks>
    /// Changed properties need to be explicitly made persistant by
    /// calling <see cref="Save()"/>.
    /// </remarks>
    public static class Settings
    {
        public static string User
        {
            get { return UserSettings.UserSettingsBase.Default.User; }
            set
            {
                UserSettings.UserSettingsBase.Default.User = value;
            }
        }

        public static string Email
        {
            get { return UserSettings.UserSettingsBase.Default.Email; }
            set
            {
                UserSettings.UserSettingsBase.Default.Email = value;
            }
        }

        public static bool CcUser
        {
            get { return UserSettings.UserSettingsBase.Default.CcUserOnExceptionReport; }
            set
            {
                UserSettings.UserSettingsBase.Default.CcUserOnExceptionReport = value;
            }
        }

        public static void Save()
        {
            UserSettings.UserSettingsBase.Default.Save();
        }
    }
}
