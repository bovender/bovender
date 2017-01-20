﻿/* LogFile.cs
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
using NLog;
using NLog.Config;
using NLog.Targets;
using NLog.Targets.Wrappers;

namespace Bovender.Logging
{
    /// <summary>
    /// Provides logging to file and to the debug console; wraps
    /// NLog configuration and targets.
    /// </summary>
    public class LogFile
    {
        #region Singleton

        /// <summary>
        /// Provides a function to generate a custom LogFile object.
        /// </summary>
        public static Func<LogFile> LogFileProvider { get; set; }

        public static LogFile Default { get { return _lazy.Value; } }

        private static readonly Lazy<LogFile> _lazy = new Lazy<LogFile>(
            () =>
            {
                LogFile logFile = null;
                if (LogFileProvider != null)
                {
                    logFile = LogFileProvider();
                }
                if (logFile == null)
                {
                    logFile = new LogFile();
                }
                return logFile;
            });

        #endregion

        #region Static properties

        /// <summary>
        /// Gets whether file logging is enabled, without initializing
        /// the singleton instance if it isn't.
        /// </summary>
        public static bool IsInitializedAndEnabled
        {
            get
            {
                return _lazy.IsValueCreated && Default.IsFileLoggingEnabled;
            }
        }

        #endregion

        #region Properties

        public bool IsFileLoggingEnabled
        {
            get
            {
                return _fileLoggingEnabled;
            }
            set
            {
                if (value != _fileLoggingEnabled)
                {
                    if (value)
                    {
                        EnableFileLogging();
                    }
                    else
                    {
                        DisableFileLogging();
                    }
                }
            }
        }

        public NLog.LogLevel LogLevel
        {
            get
            {
                return _logLevel;
            }
            set
            {
                Logger.Info("LogLevel_set: {0} ==> {1}", _logLevel, value);
                if (_fileRule != null)
                {
                    // _fileRule.DisableLoggingForLevel(_logLevel);
                    _fileRule.EnableLoggingForLevels(LogLevel.Trace, value);
                }
                _logLevel = value;
            }
        }

        /// <summary>
        /// Gets the folder where log files are stored.
        /// </summary>
        public virtual string LogFolder
        {
            get
            {
                if (_logFolder == null)
                {
                    _logFolder = System.IO.Path.GetTempPath();
                }
                return _logFolder;
            }
        }

        /// <summary>
        /// Gets the complete path and file name of the 'current' log file.
        /// </summary>
        public string CurrentLogFile
        {
            get
            {
                if (_currentLogPath == null)
                {
                    _currentLogPath = System.IO.Path.Combine(LogFolder, LOG_FILE_NAME);
                }
                return _currentLogPath;
            }
        }

        /// <summary>
        /// Gets the complete path and file name layout for the archive files.
        /// </summary>
        public virtual string ArchivedLogsPath
        {
            get
            {
                if (_archivedLogsPath == null)
                {
                    _archivedLogsPath = System.IO.Path.Combine(LogFolder, ARCHIVE_FILE_NAME);
                }
                return _archivedLogsPath;
            }
        }

        /// <summary>
        /// Gets the contents of the current log file. Returns an error
        /// string if the log file could not be read (e.g. if it does not
        /// exist).
        /// </summary>
        public string CurrentLog
        {
            get
            {
                try
                {
                    return System.IO.File.ReadAllText(CurrentLogFile);
                }
                catch (System.IO.IOException e)
                {
                    return e.Message;
                }
            }
        }

        /// <summary>
        /// Gets whether the 'current' log file is available,
        /// i.e. if the logfile exists. If loggint is disabled,
        /// this file contains the information when file logging
        /// was enabled last.
        /// </summary>
        public bool IsCurrentLogAvailable
        {
            get
            {
                return System.IO.File.Exists(CurrentLogFile);
            }
        }

        #endregion

        #region Constructor

        protected LogFile()
        {
            _logLevel = NLog.LogLevel.Info;
            _config = new LoggingConfiguration();
            LogManager.Configuration = _config;
        }

        #endregion

        #region Public methods

        public void ShowFolderInExplorer()
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(LogFolder));
        }

        public void EnableDebugLogging()
        {
            if (!_debugLoggingEnabled)
            {
                _debugLoggingEnabled = true;
                DebuggerTarget t = new DebuggerTarget();
                _config.AddTarget(DEBUG_TARGET, t);
                _config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, t));
                LogManager.ReconfigExistingLoggers();
            }
        }

        #endregion

        #region Protected properties and methods

        protected void EnableFileLogging()
        {
            _fileLoggingEnabled = true;
            if (_fileTarget == null)
            {
                _fileTarget = new FileTarget();
                _fileTarget.FileName = CurrentLogFile;
                _fileTarget.ArchiveFileName = ArchivedLogsPath;
                _fileTarget.ArchiveNumbering = ArchiveNumberingMode.Date;
                _fileTarget.ArchiveDateFormat = ARCHIVE_DATE_FORMAT;
                _fileTarget.ArchiveEvery = FileArchivePeriod.Day;
                _fileTarget.MaxArchiveFiles = MAX_ARCHIVE_FILES;
                AsyncTargetWrapper wrapper = new AsyncTargetWrapper(_fileTarget);
                _config.AddTarget(FILE_TARGET, wrapper);
            }
            if (_fileRule == null)
            {
                _fileRule = new LoggingRule("*", LogLevel, _fileTarget);
            }
            _config.LoggingRules.Add(_fileRule);
            LogManager.ReconfigExistingLoggers();
            Logger.Info("===== Begin file logging =====");
        }

        protected void DisableFileLogging()
        {
            Logger.Info("Disabling file logging");
            _config.LoggingRules.Remove(_fileRule);
            LogManager.ReconfigExistingLoggers();
            _fileLoggingEnabled = false;
        }

        #endregion

        #region Private fields

        private string _logFolder;
        private string _currentLogPath;
        private string _archivedLogsPath;
        private bool _debugLoggingEnabled;
        private bool _fileLoggingEnabled;
        private NLog.LogLevel _logLevel;
        private LoggingConfiguration _config;
        private FileTarget _fileTarget;
        private LoggingRule _fileRule;

        #endregion

        #region Private constants

        private const string FILE_TARGET = "file";
        private const string DEBUG_TARGET = "debug";
        private const string LOG_FILE_NAME = "current-log.txt";
        private const string ARCHIVE_FILE_NAME = "log-{#}.txt";
        private const string ARCHIVE_DATE_FORMAT = "yyyy-MM-dd";
        private const int MAX_ARCHIVE_FILES = 7;

        #endregion

        #region Class logger

        private static NLog.Logger Logger { get { return _logger.Value; } }

        private static readonly Lazy<NLog.Logger> _logger = new Lazy<NLog.Logger>(() => NLog.LogManager.GetCurrentClassLogger());

        #endregion
    }
}
