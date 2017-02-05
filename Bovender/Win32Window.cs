/* Win32Window.cs
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Forms = System.Windows.Forms;
using System.Windows.Interop;

namespace Bovender
{
    /// <summary>
    /// Provides access to the window handle of a WPF window or a Form
    /// or a custom handle.
    /// </summary>
    public class Win32Window : Forms.IWin32Window
    {
        #region Public static delegate

        /// <summary>
        /// Gets or sets a function that provides the global window handle of
        /// the main window of an application.
        /// </summary>
        /// <remarks>
        /// Window handles may change during the life time of an application.
        /// Excel for example will have one handle after startup, and a new
        /// main window handle when a workbook is opened.
        /// </remarks>
        public static Func<IntPtr> MainWindowHandleProvider { get; set; }

        #endregion

        #region Public property
        
        /// <summary>
        /// Gets the handle that is wrapped by this class.
        /// </summary>
        public IntPtr Handle
        {
            get
            {
                IntPtr h = _handle;
                if (h == IntPtr.Zero)
                {
                    if (MainWindowHandleProvider == null)
                    {
                        Logger.Debug("No handle has been set, and no handle provider exists");
                        h = IntPtr.Zero;
                    }
                    else
                    {
                        Logger.Debug("Obtaining handle from MainWindowHandleProvider");
                        h = MainWindowHandleProvider();
                    }
                }
                Logger.Info("Using main window handle 0x{0:X08}", h.ToInt64()); // Convert to 64-bit just to make sure...
                return h;
            }
        }

        #endregion

        #region Constructors
        
        /// <summary>
        /// Creates a new instance whose handle must be provided by the
        /// <see cref="MainWindowHandleProvider"/>
        /// </summary>
        public Win32Window()
        {
            Logger.Debug("Creating new Win32Window instance that uses the MainWindowHandleProvider");
        }

        /// <summary>
        /// Creates a new instance whose handle is obtained from a Form.
        /// </summary>
        public Win32Window(Forms.Form form)
        {
            _handle = form.Handle;
            Logger.Debug("Creating new Win32Window instance with the form handle 0x{0:X08}", _handle.ToInt64());
        }

        /// <summary>
        /// Creates a new instance whose handle is obtained from a WPF window.
        /// </summary>
        /// <param name="window"></param>
        public Win32Window(Window window)
        {
            _handle = (new WindowInteropHelper(window)).Handle;
            Logger.Debug("Creating new Win32Window instance with the WPF window handle 0x{0:X08}", _handle.ToInt64());
        }

        /// <summary>
        /// Creates a new instance that wraps a <paramref name="handle"/>.
        /// </summary>
        public Win32Window(IntPtr handle)
        {
            _handle = handle;
            Logger.Debug("Creating new Win32Window instance with the WPF window handle 0x{0:X08}", _handle.ToInt64());
        }
        
        #endregion

        #region Private fields
        
        private IntPtr _handle;

        #endregion

        #region Class logger

        private static NLog.Logger Logger { get { return _logger.Value; } }

        private static readonly Lazy<NLog.Logger> _logger = new Lazy<NLog.Logger>(() => NLog.LogManager.GetCurrentClassLogger());

        #endregion
    }
}
