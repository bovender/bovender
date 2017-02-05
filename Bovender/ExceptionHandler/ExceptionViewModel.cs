/* ExceptionViewModel.cs
 * part of Daniel's XL Toolbox NG
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using System.Collections.Specialized;
using Microsoft.Win32;
using Bovender.Mvvm;
using Bovender.Mvvm.Messaging;
using Bovender.Mvvm.ViewModels;

namespace Bovender.ExceptionHandler
{
    /// <summary>
    /// Provides easy access to several system properties that are
    /// relevant for bug reports. 
    /// </summary>
    public abstract class ExceptionViewModel : ViewModelBase
    {
        #region Public properties

        public string User
        {
            get { return _user; }
            set
            {
                _user = value;
                OnPropertyChanged("User");
            }
        }

        public string Email
        {
            get { return _email; }
            set
            {
                _email = value;
                OnPropertyChanged("Email");
                OnPropertyChanged("IsCcUserEnabled");
            }
        }

        public bool CcUser
        {
            get { return _ccUser; }
            set
            {
                _ccUser = value;
                OnPropertyChanged("CcUser");
            }
        }

        public bool IsCcUserEnabled
        {
            get
            {
                // TODO: Check if it is really an e-mail address
                        return !String.IsNullOrEmpty(Email);
            }
        }

        public string Comment
        {
            get
            {
                return _comment;
            }
            set
            {
                _comment = value;
                OnPropertyChanged("Comment");
            }
        }

        public string Exception { get; private set; }
        public string Message { get; private set; }
        public string InnerException { get; private set; }
        public string InnerMessage { get; private set; }

        public bool HasInnerException
        {
            get
            {
                return !String.IsNullOrEmpty(InnerException);
            }
        }

        public string StackTrace { get; private set; }

        public string OS
        {
            get
            {
                return Environment.OSVersion.VersionString;
            }
        }

        public string CLR
        {
            get
            {
                return Environment.Version.ToString();
            }
        }

        public string VstoRuntime
        {
            get
            {
                if (String.IsNullOrEmpty(_vstoRuntime))
                {
                    string suffix = String.Empty;
                    RegistryKey hive = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
                    RegistryKey v4 = hive.OpenSubKey(@"Software\Microsoft\VSTO Runtime Setup\V4");
                    if (v4 == null)
                    {
                        v4 = hive.OpenSubKey(@"Software\Microsoft\VSTO Runtime Setup\V4R");
                        suffix = " (v4R)";
                    }
                    if (v4 != null)
                    {
                        _vstoRuntime = v4.GetValue("Version") as string;
                    }
                    if (String.IsNullOrEmpty(_vstoRuntime))
                    {
                        _vstoRuntime = "(n/a)";
                    }
                    else
                    {
                        _vstoRuntime += suffix;
                    }
                }
                return _vstoRuntime;
            }
        }

        public string ProcessBitness
        {
            get
            {
                return Environment.Is64BitProcess ? "64-bit" : "32-bit";
            }
        }

        public string OSBitness
        {
            get
            {
                return Environment.Is64BitOperatingSystem ? "64-bit" : "32-bit";
            }
        }

        public string ReportID { get; private set; }

        public string IssueUrl { get; private set; }

        public string BovenderFramework
        {
            get
            {
                return typeof(ExceptionViewModel).Assembly.GetName().Version.ToString();
            }
        }

        public bool IsClickOnceDeployed
        {
            get
            {
                return System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed;
            }
        }

        #endregion

        #region Commands

        public DelegatingCommand SubmitReportCommand
        {
            get
            {
                if (_submitReportCommand == null)
                {
                    _submitReportCommand = new DelegatingCommand(
                        (param) => DoSubmitReport(),
                        (param) => CanSubmitReport()
                        );
                }
                return _submitReportCommand;
            }
        }

        public DelegatingCommand ViewDetailsCommand
        {
            get
            {
                if (_viewDetailsCommand == null)
                {
                    _viewDetailsCommand = new DelegatingCommand(
                        (param) => ViewDetailsMessage.Send(
                            new ViewModelMessageContent(this),
                            null)
                        );
                }
                return _viewDetailsCommand;
            }
        }

        public DelegatingCommand ClearFormCommand
        {
            get {
            if (_clearFormCommand == null) {
                _clearFormCommand = new DelegatingCommand(
                    (param) => DoClearForm(),
                    (param) => CanClearForm()
                    );
            }
            return _clearFormCommand;
            }

        }

        public DelegatingCommand NavigateIssueUrlCommand
        {
            get
            {
                if (_navigateIssueUrlCommand == null)
                {
                    _navigateIssueUrlCommand = new DelegatingCommand(
                        (param) => DoNavigateIssueUrl());
                }
                return _navigateIssueUrlCommand;
            }
        }
        #endregion

        #region MVVM messages

        /// <summary>
        /// Signals that more details about the exception are requested to be shown.
        /// </summary>
        public Message<ViewModelMessageContent> ViewDetailsMessage
        {
            get
            {
                if (_viewDetailsMessage == null)
                {
                    _viewDetailsMessage = new Message<ViewModelMessageContent>();
                }
                return _viewDetailsMessage;
            }
        }

        /// <summary>
        /// Signals that an exception report is being posted to the online
        /// issue tracker.
        /// </summary>
        public Message<MessageContent> SubmitReportMessage
        {
            get
            {
                if (_submitReportMessage == null)
                {
                    _submitReportMessage = new Message<MessageContent>();
                }
                return _submitReportMessage;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Instantiates the class and sets the report ID to the hexadecimal
        /// representation of the current ticks (time elapsed since 1 AD).
        /// </summary>
        public ExceptionViewModel(Exception e)
        {
            if (e != null)
            {
                ReportID = FileHelpers.Sha256Hash(e);

                string devPath = DevPath();
                if (!String.IsNullOrWhiteSpace(devPath))
                {
                    this.Exception = Regex.Replace(e.ToString(), devPath, String.Empty);
                    if (!String.IsNullOrEmpty(e.StackTrace))
                    {
                        StackTrace = Regex.Replace(e.StackTrace, devPath, String.Empty);
                    }
                    else
                    {
                        StackTrace = String.Empty;
                    }
                }
                Message = e.Message;
                if (e.InnerException != null)
                {
                    if (!String.IsNullOrWhiteSpace(devPath))
                    {
                        InnerException = Regex.Replace(e.InnerException.ToString(), devPath, String.Empty);
                    }
                    InnerMessage = e.InnerException.Message;
                }
                else
                {
                    InnerException = "";
                    InnerMessage = "";
                }
            }
            User = UserSettings.User;
            Email = UserSettings.Email;
            CcUser = UserSettings.CcUserOnExceptionReport;
        }

        #endregion

        #region Abstract methods

        /// <summary>
        /// Helper methods that returns a URI to POST the exception report to.
        /// </summary>
        /// <returns>Valid URI of a server that accepts POST requests.</returns>
        protected abstract Uri GetPostUri();

        protected abstract Bovender.UserSettings.UserSettingsBase UserSettings { get;  }

        #endregion

        #region Overrides

        protected override void DoCloseView()
        {
            UserSettings.User = User;
            UserSettings.Email = Email;
            UserSettings.CcUserOnExceptionReport = CcUser;
            base.DoCloseView();
        }

        #endregion

        #region Private methods

        private void webClient_UploadValuesCompleted(object sender, UploadValuesCompletedEventArgs e)
        {
            // Set 'IsIndeterminate' to false to stop the ProgressBar animation.
            SubmissionProcessMessageContent.IsIndeterminate = false;
            SubmissionProcessMessageContent.WasSuccessful = false;
            Logger.Info("Exception submission completed...");
            if (!e.Cancelled)
            {
                SubmissionProcessMessageContent.WasCancelled = false;
                if (e.Error == null)
                {
                    string result = null;
                    try
                    {
                        result = Encoding.UTF8.GetString(e.Result);
                        // Because System.Web.Helpers is not available on every system (dispite referencing
                        // the System.Web.Helpers.dll assembly), we 'parse' the simple Json result ourselves.
                        Match m = Regex.Match(result,
                            @"{\s*""ReportId"":\s*""(?<reportid>[^""]+)"",\s*""IssueUrl"":\s*""(?<issueurl>[^""]+)""\s*}");
                        if (m.Success && m.Groups["reportid"].Value == ReportID)
                        {
                            IssueUrl = m.Groups["issueurl"].Value;
                            Logger.Info("issueUrl: {0}", IssueUrl);
                            SubmissionProcessMessageContent.WasSuccessful = true;
                        }
                        else
                        {
                            throw new UnexpectedResponseException(
                                String.Format(
                                    "Received an unexpected return value from the web service (should be report ID {0}).",
                                    ReportID
                                )
                            );
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Fatal("... but response cannot be interpreted!");
                        Logger.Fatal(ex);
                        Logger.Fatal("Response: {0}", result);
                        SubmissionProcessMessageContent.Exception = new ExceptionSubmissionException(
                            "Exception submission failed", ex);
                    }
                }
                else
                {
                    Logger.Warn("... with network error:");
                    Logger.Warn(e.Error);
                    SubmissionProcessMessageContent.Exception = e.Error;
                }
            }
            else
            {
                Logger.Info("... was cancelled.");
                SubmissionProcessMessageContent.WasCancelled = true;
            }
            SubmissionProcessMessageContent.Processing = false;
            // Notify any subscribed views that the process is completed.
            SubmissionProcessMessageContent.CompletedMessage.Send(SubmissionProcessMessageContent);
        }

        private void CancelSubmission()
        {
            if (_webClient != null)
            {
                _webClient.CancelAsync();
            }
        }

        #endregion

        #region Protected methods

        protected virtual void DoSubmitReport()
        {
            Logger.Info("Submitting exception report");
            SubmissionProcessMessageContent.CancelProcess = new Action(CancelSubmission);
            SubmissionProcessMessageContent.Processing = true;
            _webClient = new WebClient();
            NameValueCollection v = GetPostValues();
            _webClient.UploadValuesCompleted += webClient_UploadValuesCompleted;
            _webClient.UploadValuesAsync(GetPostUri(), v);
            SubmitReportMessage.Send(SubmissionProcessMessageContent);
        }

        protected virtual bool CanSubmitReport()
        {
            return ((GetPostUri() != null) && !SubmissionProcessMessageContent.Processing);
        }

        protected virtual void DoClearForm()
        {
            User = String.Empty;
            Email = String.Empty;
            Comment = String.Empty;
            CcUser = true;
        }

        protected virtual bool CanClearForm()
        {
            return !(
                String.IsNullOrEmpty(User) &&
                String.IsNullOrEmpty(Email) &&
                String.IsNullOrEmpty(Comment)
                );
        }

        protected virtual void DoNavigateIssueUrl()
        {
            Logger.Info("Navigating to issue URL: {0}", IssueUrl);
            System.Diagnostics.Process.Start(IssueUrl);
            DoCloseView();
        }

        /// <summary>
        /// Returns a collection of key-value pairs of exception context information
        /// that will be submitted to the exception reporting server.
        /// </summary>
        /// <returns>Collection of key-value pairs with exception context information</returns>
        protected virtual NameValueCollection GetPostValues()
        {
            NameValueCollection v = new NameValueCollection(20);
            v["report_id"] = ReportID;
            v["usersName"] = User;
            v["usersMail"] = Email;
            v["ccUser"] = CcUser.ToString();
            v["exception"] = Exception;
            v["message"] = Message;
            v["comment"] = Comment;
            v["inner_exception"] = InnerException;
            v["inner_message"] = InnerMessage;
            v["stack_trace"] = StackTrace;
            v["process_bitness"] = ProcessBitness;
            v["operating_system"] = OS;
            v["os_bitness"] = OSBitness;
            v["clr_version"] = CLR;
            v["vstor_version"] = VstoRuntime;
            v["bovender_version"] = BovenderFramework;
            v["click_once"] = IsClickOnceDeployed.ToString();
            return v;
        }

        /// <summary>
        /// Returns the path(s) on the development machine that shall be stripped
        /// from the file information in the exception and stack trace. The return value
        /// of this function is used as the pattern in a Regex.Replace() call.
        /// </summary>
        /// <remarks>
        /// If an application is distributed along with .pdb files, the entire path of
        /// files on the development machine is included in an exception message. Since
        /// pdb files are required in order to get the line on which an exception occurred,
        /// this method provides a way to define which part of the path shall be removed.
        /// </remarks>
        /// <example>
        /// x:\XLToolbox\NG
        /// </example>
        /// <returns>String.Empty by default; derived classes should override this.</returns>
        protected virtual string DevPath()
        {
            return String.Empty;
        }

        #endregion

        #region Protected properties

        protected ProcessMessageContent SubmissionProcessMessageContent
        {
            get
            {
                if (_submissionProcessMessageContent == null)
                {
                    _submissionProcessMessageContent = new ProcessMessageContent(
                        this,
                        new Action(CancelSubmission)
                        );
                    _submissionProcessMessageContent.IsIndeterminate = true;
                }
                return _submissionProcessMessageContent;
            }
        }

        #endregion

        #region Private fields

        private string _user;
        private string _email;
        private string _comment;
        private bool _ccUser;
        private WebClient _webClient;
        private DelegatingCommand _submitReportCommand;
        private DelegatingCommand _viewDetailsCommand;
        private DelegatingCommand _clearFormCommand;
        private DelegatingCommand _navigateIssueUrlCommand;
        private Message<MessageContent> _submitReportMessage;
        private Message<ViewModelMessageContent> _viewDetailsMessage;
        private ProcessMessageContent _submissionProcessMessageContent;
        private string _vstoRuntime;

        #endregion

        #region Class logger

        private static NLog.Logger Logger { get { return _logger.Value; } }

        private static readonly Lazy<NLog.Logger> _logger = new Lazy<NLog.Logger>(() => NLog.LogManager.GetCurrentClassLogger());

        #endregion
    }
}
