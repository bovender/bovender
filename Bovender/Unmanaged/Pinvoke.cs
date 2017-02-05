/* Pinvoke.cs
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
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Bovender.Unmanaged
{
    /// <summary>
    /// Wrappers for Win32 API calls.
    /// </summary>
    public static class Pinvoke
    {
        #region Public methods

        public static void OpenClipboard(IntPtr hWndNewOwner)
        {
            int attempts = 0;
            bool opened = Win32_OpenClipboard(hWndNewOwner);
            while (!opened && attempts < CLIPBOARD_MAX_ATTEMPTS)
            {
                attempts++;
                System.Threading.Thread.Sleep(CLIPBOARD_WAIT_MS * attempts);
                opened = Win32_OpenClipboard(hWndNewOwner);
            }
            if (!opened && !Win32_OpenClipboard(hWndNewOwner))
            {
                // Compute total duration: https://en.wikipedia.org/wiki/Triangular_number
                string s = String.Format(
                    "Unable to get clipboard access; it is still locked by another application even after {0} attempts over {1:0.0} seconds",
                    attempts, CLIPBOARD_WAIT_MS * attempts * (attempts + 1) / 2 / 1000);
                Logger.Fatal(s);
                throw new Win32Exception(Marshal.GetLastWin32Error(), s);
            }
        }

        public static void CloseClipboard()
        {
            Win32_CloseClipboard();
        }

        public static IntPtr GetClipboardData(uint uFormat)
        {
             IntPtr result = Win32_GetClipboardData(uFormat);
             if (result == IntPtr.Zero)
             {
                 throw new Win32Exception(Marshal.GetLastWin32Error());
             }
             return result;
        }

        public static IntPtr CopyEnhMetaFile(IntPtr hemfSrc, string lpszFile)
        {
            IntPtr result = Win32_CopyEnhMetaFile(hemfSrc, lpszFile);
            if (result == IntPtr.Zero)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            return result;
        }

        public static void DeleteEnhMetaFile(IntPtr hemf)
        {
            Win32_DeleteEnhMetaFile(hemf);
        }

        public static string GetColorDirectory()
        {
            uint bufSize = 260; // MAX_PATH
            StringBuilder sb = new StringBuilder((int)bufSize);
            if (Win32_GetColorDirectory(IntPtr.Zero, sb, ref bufSize))
            {
                return sb.ToString();
            }
            else
            {
                return String.Empty;
            }
        }

        public static IntPtr FindWindow(string className)
        {
            return FindWindow(className, null);
        }

        #endregion

        #region Win32 API constants

        public const uint CF_ENHMETAFILE = 14;

        #endregion

        #region Win32 DLL imports

        [DllImport("user32.dll", EntryPoint = "OpenClipboard", SetLastError = true)]
        static extern bool Win32_OpenClipboard(IntPtr hWndNewOwner);

        [DllImport("user32.dll", EntryPoint = "CloseClipboard", SetLastError = true)]
        static extern bool Win32_CloseClipboard();

        [DllImport("user32.dll", EntryPoint = "GetClipboardOwner", SetLastError = true)]
        static extern IntPtr Win32_GetClipboardOwner();

        [DllImport("user32.dll", EntryPoint = "GetClipboardData", SetLastError = true)]
        static extern IntPtr Win32_GetClipboardData(uint uFormat);

        [DllImport("gdi32.dll", EntryPoint = "CopyEnhMetaFile", SetLastError = true)]
        static extern IntPtr Win32_CopyEnhMetaFile(IntPtr hemfSrc, string lpszFile);

        [DllImport("gdi32.dll", EntryPoint = "DeleteEnhMetaFile", SetLastError = true)]
        static extern bool Win32_DeleteEnhMetaFile(IntPtr hemf);

        [DllImport("mscms.dll", EntryPoint = "GetColorDirectory", SetLastError = true,
            CharSet = CharSet.Auto, BestFitMapping = false)]
        static extern bool Win32_GetColorDirectory(IntPtr pMachineName, StringBuilder pBuffer,
            ref uint pdwSize);

        /// <summary>
        ///     Retrieves a handle to the top-level window whose class name and window name match the specified strings. This
        ///     function does not search child windows. This function does not perform a case-sensitive search. To search child
        ///     windows, beginning with a specified child window, use the
        ///     <see cref="!:https://msdn.microsoft.com/en-us/library/windows/desktop/ms633500%28v=vs.85%29.aspx">FindWindowEx</see>
        ///     function.
        ///     <para>
        ///     Go to https://msdn.microsoft.com/en-us/library/windows/desktop/ms633499%28v=vs.85%29.aspx for FindWindow
        ///     information or https://msdn.microsoft.com/en-us/library/windows/desktop/ms633500%28v=vs.85%29.aspx for
        ///     FindWindowEx
        ///     </para>
        /// </summary>
        /// <param name="lpClassName">
        ///     C++ ( lpClassName [in, optional]. Type: LPCTSTR )<br />The class name or a class atom created by a previous call to
        ///     the RegisterClass or RegisterClassEx function. The atom must be in the low-order word of lpClassName; the
        ///     high-order word must be zero.
        ///     <para>
        ///     If lpClassName points to a string, it specifies the window class name. The class name can be any name
        ///     registered with RegisterClass or RegisterClassEx, or any of the predefined control-class names.
        ///     </para>
        ///     <para>If lpClassName is NULL, it finds any window whose title matches the lpWindowName parameter.</para>
        /// </param>
        /// <param name="lpWindowName">
        ///     C++ ( lpWindowName [in, optional]. Type: LPCTSTR )<br />The window name (the window's
        ///     title). If this parameter is NULL, all window names match.
        /// </param>
        /// <returns>
        ///     C++ ( Type: HWND )<br />If the function succeeds, the return value is a handle to the window that has the
        ///     specified class name and window name. If the function fails, the return value is NULL.
        ///     <para>To get extended error information, call GetLastError.</para>
        /// </returns>
        /// <remarks>
        /// <para>
        ///     If the lpWindowName parameter is not NULL, FindWindow calls the <see cref="M:GetWindowText" /> function to
        ///     retrieve the window name for comparison. For a description of a potential problem that can arise, see the Remarks
        ///     for <see cref="M:GetWindowText" />.
        /// </para>
        /// <para>
        ///     From: http://www.pinvoke.net/default.aspx/user32.findwindow
        /// </para>
        /// </remarks>
        // For Windows Mobile, replace user32.dll with coredll.dll
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        // Find window by Caption only. Note you must pass IntPtr.Zero as the first parameter.
        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);

        // You can also call FindWindow(default(string), lpWindowName) or FindWindow((string)null, lpWindowName)
        #endregion

        #region Private constants

        private const int CLIPBOARD_MAX_ATTEMPTS = 5;
        private const int CLIPBOARD_WAIT_MS = 200;

        #endregion

        #region Class logger

        private static NLog.Logger Logger { get { return _logger.Value; } }

        private static readonly Lazy<NLog.Logger> _logger = new Lazy<NLog.Logger>(() => NLog.LogManager.GetCurrentClassLogger());

        #endregion
    }
}
