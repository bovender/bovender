/* WindowExtensions.cs
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
using System.Windows;
using System.Windows.Interop;

namespace Bovender.Extensions
{
    /// <summary>
    /// Extension methods for the WPF Window class.
    /// </summary>
    public static class WindowExtensions
    {
        #region Global static property

        /// <summary>
        /// May hold the HWND of a Windows forms top-level window; if set, it
        /// is used to set a WPF window's owner by ViewModels that inject
        /// themselves into a WPF View.
        /// </summary>
        public static IntPtr TopLevelWindow { get; set; }

        #endregion

        #region Public extension methods for Window

        /// <summary>
        /// Make a Windows Form the Owner of a WPF Window.
        /// </summary>
        /// <param name="ownerForm">Windows Forms handle that will be used as
        /// the owner of the WPF window.</param>
        public static void SetOwnerForm(this Window window, IntPtr ownerForm)
        {
            // This method should not throw an exception when ownerForm is zero.
            if (ownerForm != IntPtr.Zero)
            {
                WindowInteropHelper h = new WindowInteropHelper(window);
                h.Owner = ownerForm;
            }
        }

        /// <summary>
        /// Shows the Window as a dialog that belongs to a Windows form parent.
        /// </summary>
        public static bool? ShowDialogInForm(this Window window, IntPtr parentForm)
        {
            window.SetOwnerForm(parentForm);
            return window.ShowDialog();
        }

        /// <summary>
        /// Shows the Window as a dialog that belongs to a Windows form parent.
        /// This overload requires the TopLevelWindow property to be set.
        /// </summary>
        public static bool? ShowDialogInForm(this Window window)
        {
            if (TopLevelWindow == IntPtr.Zero)
            {
                throw new InvalidOperationException("Must set TopLevelWindow property before calling this method.");
            }
            return window.ShowDialogInForm(TopLevelWindow);
        }

        /// <summary>
        /// Shows the Window as a dialog that belongs to a Windows form parent.
        /// </summary>
        public static void ShowInForm(this Window window, IntPtr parentForm)
        {
            if (parentForm != IntPtr.Zero)
            {
                WindowInteropHelper h = new WindowInteropHelper(window);
                // TODO
                h.Owner = TopLevelWindow;
            }
            window.Show();
        }

        /// <summary>
        /// Shows the Window as a dialog that belongs to a Windows form parent.
        /// </summary>
        /// <param name="window">WPF window who's Windows forms owner to set.</param>
        public static void ShowInForm(this Window window)
        {
            window.ShowInForm(TopLevelWindow);
        }

        #endregion
    }
}
