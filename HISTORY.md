Version 0.4.0 (2015-09-28)
========================================================================

- CHANGE: PercentCompleted property of ProcessMessageContent to int (because the ProgressBar expects a number between 0 and 100, rather than 0.0 and 1.0).
- FIX: ViewModelBase.InjectAndShowInThread invoked Dispatcher shutdown too early.
- NEW: Implement last-resort crash dump in central exception handler.

* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 


Version 0.3.3. (2015-09-19)
========================================================================

- FIX: Close 'update available' views when update download starts.

* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 


Version 0.3.2. (2015-09-15)
========================================================================

- FIX: Cancel button of ProcessView was missing command binding.

* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 


Version 0.3.1 (2015-09-09)
========================================================================

- FIX: WpfHelpers method was not declared public.

* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 


Version 0.3.0 (2015-09-09)
========================================================================

- IMPROVED: Update download URIs may contain the placeholder $VERSION that will be substituted with the update's version.
- NEW: static WpfHelpers class that provides a method to have WPF text boxes select all text on focus.

* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 





Version 0.1.0 (2015-06-08)
========================================================================

Initial NuGet package after extracting the framework from the XL Toolbox NG solution.


* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
