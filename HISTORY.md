Version 0.9.0 (2016-06-17)
------------------------------------------------------------------------

- IMPROVEMENT: DllManager now attempts to locate a DLL in an alternative directory as well.
- NEW: Added property for Click Once installation to ExceptionViewModel.
- NEW: Bovender.Mvvm.Models.ProcessModel for long-running processes.
- NEW: ProcessViewModelBase class that facilitates showing progress on long-running processes.
- NEW: ViewModelBase.Dispatch helper that dispatches an action of function either directly or via the associated view's dispatcher, if one exists.

* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 


Version 0.8.2 (2016-06-14)
------------------------------------------------------------------------

- FIX: Handle denial of access for the Windows clipboard more gracefully (by waiting longer for access and raising a more meaningful exception).

* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 


Version 0.8.1 (2016-06-11)
------------------------------------------------------------------------

- IMPROVED: Expect a JSON response to submitting an exception report that includes a URL to the issue page.

* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 


Version 0.8.0. (2016-06-03)
------------------------------------------------------------------------

- CHANGE: FileDialogActionBase now shows the dialogs with the owner provided by Win32Window.
- FIX: Show windows anyway even if invalid window handle exception occurred (fixes bovender/XLToolbox#7).
- NEW: P/invoke wrapper for FindWindow.
- NEW: Win32Window class that can be used to deal with main window handles.

* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 


Version 0.7.0 (2016-05-27)
------------------------------------------------------------------------

- FIX: ExceptionViewModel did not make use of the new UserSettingsBase class.
- FIX: Log message when saving UserSettingsBase.
- FIX: Prevent null reference exceptions in ExceptionViewModel constructor if no exception is given.
- IMPROVEMENT: The owner form can now be set when injecting view models into views.

* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 


Version 0.6.1 (2016-05-24)
------------------------------------------------------------------------

- FIX: When aborting an update, do not attempt to delete the partially downloaded file, because the WebClient won't leave one.

* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 


Version 0.6.0 (2016-05-16)
------------------------------------------------------------------------

- FIX: No longer crash if SemanticVersion variables with null value are compared.
- FIX: Produce SemanticVersion string from current properties (instead of just returning the original string).
- IMPROVED: Added logging and overloaded constructors of NotificationAction.
- NEW: Add logging using the NLog framework.
- NEW: Extension for Object to compute an object's MD5 checksum hash.
- NEW: Extension for String to truncate with ellipsis.

* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 


Version 0.5.2 (2016-05-06)
------------------------------------------------------------------------

- FIX: Revert the changes related to Windows Forms dialog introduced in version 0.5.1 because they did not prevent the sporadic hangs.
- IMPROVEMENT: Added Window extensions methods (ShowDialogInForm) that can set the Window's owner to a Windows Forms window.

* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 


Version 0.5.1 (2016-04-29)
------------------------------------------------------------------------

- FIX: Attempt to prevent application from hanging when it really should display a file dialog.
- FIX: Do not throw a null reference exception in Updater if an update download is cancelled (#703f87f7).

* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 


Version 0.5.0 (2015-10-18)
------------------------------------------------------------------------

- FIX: Set progress bar range in ProcessView to integers.
- IMPROVEMENT: Updater class now handles SHA-256 checksums in version info files.

* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 


Version 0.4.0 (2015-09-28)
------------------------------------------------------------------------

- CHANGE: PercentCompleted property of ProcessMessageContent to int (because the ProgressBar expects a number between 0 and 100, rather than 0.0 and 1.0).
- FIX: ViewModelBase.InjectAndShowInThread invoked Dispatcher shutdown too early.
- NEW: Implement last-resort crash dump in central exception handler.

* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 


Version 0.3.3. (2015-09-19)
------------------------------------------------------------------------

- FIX: Close 'update available' views when update download starts.

* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 


Version 0.3.2. (2015-09-15)
------------------------------------------------------------------------

- FIX: Cancel button of ProcessView was missing command binding.

* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 


Version 0.3.1 (2015-09-09)
------------------------------------------------------------------------

- FIX: WpfHelpers method was not declared public.

* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 


Version 0.3.0 (2015-09-09)
------------------------------------------------------------------------

- IMPROVED: Update download URIs may contain the placeholder $VERSION that will be substituted with the update's version.
- NEW: static WpfHelpers class that provides a method to have WPF text boxes select all text on focus.

* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 





Version 0.1.0 (2015-06-08)
------------------------------------------------------------------------

Initial NuGet package after extracting the framework from the XL Toolbox NG solution.

* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 


Documentation for version 0.8.0. (2016-06-03)
------------------------------------------------------------------------



* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
