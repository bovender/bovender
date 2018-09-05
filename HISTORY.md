Version 0.17.2 (2018-09-05)
------------------------------------------------------------------------

- Improved: Added a static property WpfHelpers.MainDispatcher to facility storing a dispatcher for the main UI thread.

* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 


Version 0.17.1 (2018-08-14)
------------------------------------------------------------------------

- Fix: Prevent race condition when checking for new release.
- Fix: Semantic versions with pre-release information were not compared properly.

* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 


Version 0.17.0 (2017-03-03)
------------------------------------------------------------------------

- Change: List items in Style.xaml receive some top and bottom padding.
- Improvement: The UserSettingsBase class now provides virtual methods that can provide YamlDotNet SerializerBuilder objects with custom properties.
- Improvement: Use updated YamlDotNet (4.1.0) and NUnit packages.

* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 


Version 0.16.2 (2017-02-15)
------------------------------------------------------------------------

- Fix: Rebuild binaries.

* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 


Version 0.16.1 (2017-02-05)
------------------------------------------------------------------------

- Change: Exception IDs are now SAH-256 checksum hashes of an exception.
- Fix: Error messages from processes are now wrapped.
- Fix: Exception wrapping in ProcessFailedView.
- Fix: Log levels were not set correctly.

* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 


Version 0.16.0 (2016-12-22)
------------------------------------------------------------------------

- Improvement: Better exception reporting in ProcessViewModelBase.
- New: Add ability to specify log level in LogFile.

* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 


Version 0.15.1 (2016-11-16)
------------------------------------------------------------------------

- Improvement: DLL manager now keeps track of loaded DLLs globally.
- Improvement: Write log messages when COM objects are released.

* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 


Version 0.15.0 (2016-09-22)
------------------------------------------------------------------------

- Improvement: Added a suppressible message action and view.

* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 


Version 0.14.6 (2016-09-15)
------------------------------------------------------------------------

- Fix: If file access is denied during checksum generation, up to three attempts are now made to open the file stream before an exception is raised.

* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 


Version 0.14.5 (2016-09-03)
------------------------------------------------------------------------

- FIX: Prevent fatal exception in ExceptionViewModel if an inner exception is present and dev path is null or empty.
- IMPROVEMENT: Do not throw an exception in ShowDialogInForm if there is no window handle provider.

* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 


Version 0.14.4 (2016-09-03)
------------------------------------------------------------------------

- FIX: Version number.

* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 


Version 0.14.3 (2016-09-03)
------------------------------------------------------------------------

- FIX: Uncommitted changes.

* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 


Version 0.14.2 (2016-09-03)
------------------------------------------------------------------------

- UPDATE: Nuget packages: NLog 4.3.7 and YamlDotNet 3.9.0.

* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 


Version 0.14.1 (2016-08-30)
------------------------------------------------------------------------

- CHANGE: Removed ReleaseComObject extension method and placed it in a new ComHelpers static class because COM objects do not agree well with extension methods on object.
- FIX: Synchronization in ViewModelCollection.
- FIX: ViewModelCollection did not properly regenerate model collection on mass change.

* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 


Version 0.14.0 (2016-08-20)
------------------------------------------------------------------------

- IMPROVEMENT: Log error message if ShowInForm or ShowDialogInForm fails.
- NEW: Added all caps converter for WPF.
- NEW: Focus-first behavior for WPF.
- NEW: ReleaseComObject extension method for object.
- NEW: Reusable FailureSign and SuccessSign components.
- NEW: Set minimum width on all buttons to 80 in Style.xaml.

* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 


Version 0.13.0 (2016-08-12)
------------------------------------------------------------------------

- FIX: Explicitly open files for read access only when computing checksums.
- FIX: The RemoveSelected method of the ViewModelCollection no longer triggers a 'range actions not supported' exception.
- NEW: Added a VisibilityBooleanNegationConverter.

* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 


Version 0.12.1 (2016-08-02)
------------------------------------------------------------------------

- FIX: Use the correct command to install an update.

* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 


Version 0.12.0 (2016-08-01)
------------------------------------------------------------------------

- IMPROVEMENT: The automatic updater facility was overhauled, it now makes use of the ProcessModel and ProcessViewModelBase.

* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 


Version 0.11.3 (2016-07-24)
------------------------------------------------------------------------

- IMPROVEMENT: An update installer is now executed with an additional /UPDATE command line switch.
- IMPROVEMENT: Logging during updating.

* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 


Version 0.11.2 (2016-07-13)
------------------------------------------------------------------------

- IMPROVED: The ExceptionViewModel also provides access to the VSTO Runtime version now.

* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 


Version 0.11.1 (2016-07-11)
------------------------------------------------------------------------

- FIX: Invoking an MVVM action using the extension method without parameter no longer causes an exception.

* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 


Version 0.11.0 (2016-07-02)
------------------------------------------------------------------------

- FIX: Storing and retrieving SemanticVersion objects from Settings.
- IMPROVEMENT: Process-related code was refactored and simplified.
- NEW: Built-in LogFile class.

* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 


Version 0.10.0 (2016-06-20)
------------------------------------------------------------------------

- CHANGE: The MVVM MessageAction system no longer injects MessageActionBase objects into views, but ViewModelBase-derived classes.
- IMPROVEMENT: Better out-of-the-box support for process actions.
- IMPROVEMENT: The MVVM actions were rewired and changed so that they never inject themselves into views. Action view models are now used for this purpose.

* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 


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
