/* Settings.cs
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace Bovender.UserSettings
{
    /// <summary>
    /// Base class for persistent settings; a replacement for the UserSettings.UserSettingsBase
    /// system which has a couple of quirks and whose files are hard to find, read,
    /// and write by humans.
    /// </summary>
    /// <remarks>
    /// To make use of this options system, derive your own class from OptionsBase
    /// and add properties to it. These will be automatically saved to and loaded from
    /// a YAML file. 
    /// </remarks>
    public class UserSettingsBase
    {
        #region Singleton

        /// <summary>
        /// Gets the singleton instance of the user settings. Applications that
        /// use this framework may want to replace the default singleton with
        /// an instance of their own (UserSettingsBase.Default = myInstance),
        /// in order to avoid having two separate settings files, one for the
        /// Bovender framework and one for the application itself.
        /// </summary>
        public static UserSettingsBase Default
        {
            get
            {
                return _lazy.Value;
            }
            set
            {
                _lazy = new Lazy<UserSettingsBase>(() => value);
            }
        }

        private static Lazy<UserSettingsBase> _lazy = new Lazy<UserSettingsBase>(() => new UserSettingsBase());

        #endregion

        #region Factory

        /// <summary>
        /// Loads options from a file, or creates a default instance if the I/O
        /// operation failed. This is a protected method. Implementations of
        /// UserSettingsBase must provide simple-access methods defined in
        /// IUserSettings to make life easier for the outside world.
        /// </summary>
        /// <param name="yamlFile">Path to a YAML file that holds the options.</param>
        /// <typeparam name="T">Type of the OptionsStore derivative to load.</typeparam>
        /// <returns>OptionsStore object, either with values loaded from file
        /// or with default values.</returns>
        protected static T FromFileOrDefault<T>(string yamlFile)
            where T: UserSettingsBase, new()
        {
            T optionsStore = null;
            if (File.Exists(yamlFile))
            {
                try
                {
                    using (FileStream fs = File.Open(yamlFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        StreamReader sr = new StreamReader(fs);
                        Deserializer des = new Deserializer(ignoreUnmatched: true);
                        optionsStore = des.Deserialize<T>(sr);
                        if (optionsStore != null)
                        {
                            optionsStore.WasFromFile = true;
                        }
                    }
                    Logger.Info("Loaded user settings from file '{0}'", yamlFile);
                }
                catch (IOException e)
                {
                    optionsStore = CreateDefaultOnException<T>(e);
                }
                catch (YamlException e)
                {
                    optionsStore = CreateDefaultOnException<T>(e);
                }
            }
            if (optionsStore == null)
	        {
                Logger.Info("Creating user settings object from scratch");
                optionsStore = new T();
	        }
            return optionsStore;
        }

        #endregion

        #region User settings

        public string User
        {
            get
            {
                if (_user == null)
                {
                    _user = "";
                }
                return _user;
            }
            set
            {
                _user = value;
            }
        }

        public string Email
        {
            get
            {
                if (_email == null)
                {
                    _email = "";
                }
                return _email;
            }
            set
            {
                _email = value;
            }
        }

        public bool CcUserOnExceptionReport
        {
            get
            {
                if (_ccUserOnExceptionReport == null)
                {
                    _ccUserOnExceptionReport = true;
                }
                return (bool)_ccUserOnExceptionReport;
            }
            set
            {
                _ccUserOnExceptionReport = value;
            }
        }

        public string DownloadFolder
        {
            get
            {
                if (_downloadFolder == null)
                {
                    _downloadFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                }
                return _downloadFolder;
            }
            set
            {
                _downloadFolder = value;
            }
        }

        #endregion

        #region Public static methods

        /// <summary>
        /// Creates a new settings object without loading the saved settings
        /// from file and without saving the current settings from file.
        /// </summary>
        /// <remarks>
        /// Derived classes should implement their own static variant of this
        /// method (which cannot be marked virtual because it is static), in
        /// order to generate an instance of the derived class, rather than
        /// an instance of UserSettingsBase.
        /// </remarks>
        public static void LoadDefaults()
        {
            _lazy = new Lazy<UserSettingsBase>(() => new UserSettingsBase());
        }

        #endregion

        #region Properties that are not being saved to the YAML file

        /// <summary>
        /// Is true if the options store was initially loaded from a
        /// YAML file.
        /// </summary>
        [YamlIgnore]
        public bool WasFromFile { get; protected set; }

        /// <summary>
        /// Gets the last IOException, if any occurred.
        /// </summary>
        [YamlIgnore]
        public Exception Exception { get; protected set; }

        #endregion

        #region Constructor

        public UserSettingsBase() { }
        
        #endregion

        #region Save to file

        /// <summary>
        /// Saves the user settings to a file.
        /// </summary>
        public void Save()
        {
            try
            {
                string fn = GetSettingsFilePath();
                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(fn));
                using (FileStream fs = File.Open(fn, FileMode.Create, FileAccess.Write))
                {
                    StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
                    WriteYamlHeader(sw);
                    Serializer ser = new Serializer();
                    ser.Serialize(sw, this);
                    sw.Flush();
                }
                Exception = null;
                Logger.Info("Saved user settings to file '{0}'", fn);
            }
            catch (IOException e)
            {
                Logger.Warn("Could not save user settings", e);
                Exception = e;
            }
        }

        /// <summary>
        /// Gets the complete path and file name for the user settings file.
        /// </summary>
        /// <remarks>
        /// Applications using this framework should override this.
        /// </remarks>
        public virtual string GetSettingsFilePath()
        {
            return System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                Properties.Settings.Default.UserSettingsPath,
                Properties.Settings.Default.UserSettingsFile);
        }

        /// <summary>
        /// Writes a header to the YAML file before all other data. To write
        /// comments, prefix lines with "#".
        /// </summary>
        protected virtual void WriteYamlHeader(StreamWriter streamWriter)
        {
            streamWriter.WriteLine("# !!! DO NOT EDIT THIS FILE WHILE THE APPLICATION IS RUNNING !!!");
        }
        
        #endregion

        #region Private fields

        private string _user;
        private string _email;

        /// <summary>
        /// Use a nullable bool so we can set a default value of True
        /// in the property accessor.
        /// </summary>
        private bool? _ccUserOnExceptionReport;

        private string _downloadFolder;

        #endregion

        #region Private static method

        /// <summary>
        /// Helper method that creates a new options object, loads default
        /// values, and sets the Exception property. This is necessary
        /// because we need to catch both IOExceptions and YamlExceptions
        /// in the factory method FromFileOrDefault, but the common
        /// ancestor is the base Exception class, which is quite unspecific.
        /// </summary>
        private static T CreateDefaultOnException<T>(Exception e)
            where T: UserSettingsBase, new()
        {
            Logger.Warn(e, "Exception during loading of user settings, using default");
            T options = new T();
            options.Exception = e;
            return options;
        }

        #endregion

        #region Class logger

        private static NLog.Logger Logger { get { return _logger.Value; } }

        private static readonly Lazy<NLog.Logger> _logger = new Lazy<NLog.Logger>(() => NLog.LogManager.GetCurrentClassLogger());

        #endregion
    }
}
