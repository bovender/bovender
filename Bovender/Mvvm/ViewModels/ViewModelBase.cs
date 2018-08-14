/* ViewModelBase.cs
 * part of Daniel's XL Toolbox NG
 * 
 * Copyright 2014-2018 Daniel Kraus
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
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Threading;
using System.Windows.Threading;
using Bovender.Extensions;
using System.Threading.Tasks;

namespace Bovender.Mvvm.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        #region Public properties

        public virtual string DisplayString
        {
            get
            {
                return _displayString;
            }
            set
            {
                if (value != _displayString)
                {
                    _displayString = value;
                    OnPropertyChanged("DisplayString");
                }
            }
        }

        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }

        public Dispatcher ViewDispatcher { get; set; }

        #endregion

        #region Public methods

        /// <summary>
        /// Determines whether the current object is a view model
        /// of a particular model object. Returns false if either
        /// the <see cref="model"/> or the viewmodel's wrapped
        /// model object is null.
        /// </summary>
        /// <param name="model">The model to check.</param>
        /// <returns>True if <see cref="model"/> is wrapped by
        /// this; false if not (including null objects).</returns>
        public bool IsViewModelOf(object model)
        {
            object wrappedObject = RevealModelObject();
            if (model == null || wrappedObject == null)
            {
                return false;
            }
            else
            {
                return wrappedObject.Equals(model);
            }
        }

        #endregion

        #region Public injectors

        /// <summary>
        /// Injects the ViewModel into a newly created View and wires the RequestCloseView
        /// event.
        /// </summary>
        /// <typeparam name="T">View, must be derived from <see cref="System.Windows.Window"/>
        /// </typeparam>
        /// <returns>View with DataContext set to the current ViewModel instance that
        /// responds to the RequestCloseView event by closing itself.</returns>
        public Window InjectInto<T>() where T : Window, new()
        {
            T view = new T();
            return InjectInto(view);
        }

        /// <summary>
        /// Injects the view model into an existing view by setting
        /// the view's DataContext.
        /// </summary>
        /// <param name="view">View that shall be dependency injected.</param>
        /// <returns>View with current view model injected.</returns>
        public Window InjectInto(Window view)
        {
            if (view != null)
            {
                EventHandler h = null;
                h = (sender, args) =>
                {
                    this.RequestCloseView -= h;
                    view.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        view.DataContext = null;
                        view.Close();
                    }));
                };
                this.RequestCloseView += h;
                view.DataContext = this;
                ViewDispatcher = view.Dispatcher;
            }
            return view;
        }

        /// <summary>
        /// Creates a new thread that creates a new instance of the view <typeparamref name="T"/>,
        /// sets its Forms owner and shows it modelessly.
        /// Use this to show views during asynchronous operations.
        /// </summary>
        /// <param name="ownerForm">Handle of the Forms window that the view should
        /// belong to.</param>
        /// <typeparam name="T">View (descendant of Window).</typeparam>
        public void InjectAndShowInThread<T>(IntPtr ownerForm) where T: Window, new()
        {
            Thread t = new Thread(() =>
            {
                T view = new T();
                EventHandler h = null;
                h = (sender, args) =>
                {
                    this.RequestCloseView -= h;
                    // view.Close();
                    view.Dispatcher.Invoke(new Action(view.Close));
                    view.Dispatcher.InvokeShutdown();
                };
                this.RequestCloseView += h;
                ViewDispatcher = view.Dispatcher;
                view.DataContext = this;
                // Must shut down the Dispatcher, but this has been moved from
                // the Closed event of the view to the RequestCloseView event
                // of the view model in order to prevent premature termination
                // of the thread if there are other views that this view model
                // has been injected into.
                // view.Closed += (sender, args) => view.Dispatcher.InvokeShutdown();
                view.ShowInForm(ownerForm);
                System.Windows.Threading.Dispatcher.Run();
            });
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }

        /// <summary>
        /// Creates a new thread that creates a new instance of the view <typeparamref name="T"/>
        /// and shows it modelessly. Use this to show views during asynchronous operations.
        /// </summary>
        /// <typeparam name="T">View (descendant of Window).</typeparam>
        public void InjectAndShowInThread<T>() where T: Window, new()
        {
            InjectAndShowInThread<T>(IntPtr.Zero);
        }

        /// <summary>
        /// Creates a new thread that creates a new instance of the view <typeparamref name="T"/>
        /// and shows it as a dialog. Use this to show dialogs during asynchronous operations.
        /// </summary>
        /// <param name="ownerForm">Handle of the Forms window that the view should
        /// belong to.</param>
        /// <typeparam name="T">View (descendant of Window).</typeparam>
        public void InjectAndShowDialogInThread<T>(IntPtr ownerForm) where T : Window, new()
        {
            Thread t = new Thread(() =>
            {
                T view = new T();
                EventHandler h = null;
                h = (sender, args) =>
                {
                    this.RequestCloseView -= h;
                    // view.Close();
                    view.Dispatcher.Invoke(new Action(view.Close));
                    view.Dispatcher.InvokeShutdown();
                };
                this.RequestCloseView += h;
                ViewDispatcher = view.Dispatcher;
                view.DataContext = this;
                // Must shut down the Dispatcher, but this has been moved from
                // the Closed event of the view to the RequestCloseView event
                // of the view model in order to prevent premature termination
                // of the thread if there are other views that this view model
                // has been injected into.
                // view.Closed += (sender, args) => view.Dispatcher.InvokeShutdown();
                view.ShowDialogInForm(ownerForm);
                // System.Windows.Threading.Dispatcher.Run();
            });
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }

        #endregion

        #region Public (abstract) methods

        /// <summary>
        /// Returns the model object that this view model wraps or null
        /// if there is no wrapped model object.
        /// </summary>
        /// <remarks>
        /// This is a method rather than a property to make data binding
        /// more difficult (if not impossible), because binding directly
        /// to the model object is discouraged. However, certain users
        /// such as a ViewModelCollection might need access to the wrapped
        /// model object.
        /// </remarks>
        /// <returns>Model object.</returns>
        public abstract object RevealModelObject();

        #endregion

        #region Events

        /// <summary>
        /// Raised by the CloseView Command, signals that associated views
        /// are to be closed.
        /// </summary>
        public event EventHandler RequestCloseView;

        #endregion

        #region Commands

        public ICommand CloseViewCommand
        {
            get
            {
                if (_closeViewCommand == null)
                {
                    _closeViewCommand = new DelegatingCommand(
                        parameter => { DoCloseView(); },
                        parameter => { return CanCloseView(); }
                        );
                };
                return _closeViewCommand;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Does not allow public instantiation of this class.
        /// </summary>
        protected ViewModelBase()
        {
            // Capture the current dispatcher to enable
            // asynchronous operations that a view can
            // react to dispite running in another thread.
            Dispatcher = Dispatcher.CurrentDispatcher;

            if (SynchronizationContext.Current != null)
            {
                SyncContext = TaskScheduler.FromCurrentSynchronizationContext();
            }
        }

        #endregion

        #region INotifyPropertyChanged interface

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Protected methods

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected virtual bool CanCloseView()
        {
            return true;
        }

        protected virtual void DoCloseView()
        {
            if (RequestCloseView != null && CanCloseView())
            {
                RequestCloseView(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Dispatches an action in the current synchronization context
        /// if one exists, or using the Dispatcher.
        /// </summary>
        /// <param name="action">Action to dispatch</param>
        protected CancellationToken Dispatch(Action action)
        {
            if (SyncContext == null)
            {
                Logger.Info("Dispatch: Dispatching with dispatcher");
                Dispatcher.Invoke(action);
                return CancellationToken.None;
            }
            else
            {
                Logger.Info("Dispatch: Dispatching on current synchronization context");
                // TaskScheduler context = TaskScheduler.FromCurrentSynchronizationContext();
                CancellationToken token = new CancellationToken();
                Task.Factory.StartNew(action, token, TaskCreationOptions.None, SyncContext);
                return token;
            }
        }

        #endregion

        #region Protected properties

        /// <summary>
        /// Captures the dispatcher of the thread that the
        /// object was created in.
        /// </summary>
        protected Dispatcher Dispatcher { get; private set; }

        protected TaskScheduler SyncContext { get; private set; }

        #endregion

        #region Private fields

        private string _displayString;
        private DelegatingCommand _closeViewCommand;
        private bool _isSelected;

        #endregion

        #region Class logger

        private static NLog.Logger Logger { get { return _logger.Value; } }

        private static readonly Lazy<NLog.Logger> _logger = new Lazy<NLog.Logger>(() => NLog.LogManager.GetCurrentClassLogger());

        #endregion
    }
}
