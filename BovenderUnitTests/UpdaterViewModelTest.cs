using System;
using System.Threading.Tasks;
using Bovender.Versioning;
using Bovender.Mvvm.Messaging;
using NUnit.Framework;

namespace Bovender.UnitTests
{
    [TestFixture]
    public class UpdaterViewModelTest
    {
        [Test]
        public void NoUpdateAvailable()
        {
            UpdaterForTesting updater = new UpdaterForTesting();
            UpdaterViewModel vm = new UpdaterViewModel(updater);
            bool checkFinished = false;
            bool messageSent = false;
            // Add or own event handler to updater's CheckForUpdateFinished
            // event so we know when the low-level operation has been
            // completed.
            updater.CheckForUpdateFinished += (sender, args) =>
            {
                checkFinished = true;
            };
            updater.TestVersion = "99.99.99";
            vm.NoUpdateAvailableMessage.Sent += (sender, messageArgs) =>
            {
                messageSent = true;
            };
            Task checkFinishedTask = new Task(() =>
            {
                while (checkFinished == false) ;
            });
            vm.CheckForUpdateCommand.Execute(null);
            checkFinishedTask.Start();

            // Wait for the update check to complete asynchronously
            checkFinishedTask.Wait(10000);

            Task checkMessageSentTask = new Task(() =>
            {
                while (!messageSent) ;
            });

            // Give the MVVM messaging a chance to work before
            // we assert that the message has indeed been sent.
            checkMessageSentTask.Wait(1000);
            Assert.True(messageSent, "NoUpdateAvailableMessage should have been sent but wasn't.");
        }

        [Test]
        public void UpdateAvailable()
        {
            UpdaterForTesting updater = new UpdaterForTesting();
            UpdaterViewModel vm = new UpdaterViewModel(updater);
            bool checkFinished = false;
            bool messageSent = false;
            // Add or own event handler to updater's CheckForUpdateFinished
            // event so we know when the low-level operation has been
            // completed.
            updater.CheckForUpdateFinished += (sender, args) =>
            {
                checkFinished = true;
            };
            updater.TestVersion = "0.0.0";
            vm.UpdateAvailableMessage.Sent += (sender, messageArgs) =>
            {
                messageSent = true;
            };
            Task checkFinishedTask = new Task(() =>
            {
                while (checkFinished == false) ;
            });
            vm.CheckForUpdateCommand.Execute(null);
            checkFinishedTask.Start();

            // Wait for the update check to complete asynchronously
            checkFinishedTask.Wait(10000);

            Task checkMessageSentTask = new Task(() =>
            {
                while (!messageSent) ;
            });

            // Give the MVVM messaging a chance to work before
            // we assert that the message has indeed been sent.
            checkMessageSentTask.Wait(1000);
            Assert.True(messageSent, "NoUpdateAvailableMessage should have been sent but wasn't.");

        }

        [Test]
        public void DownloadUpdate()
        {
            UpdaterForTesting updater = new UpdaterForTesting();
            updater.TestVersion = "0.0.0";
            UpdaterViewModel vm = new UpdaterViewModel(updater);
            vm.UpdateAvailableMessage.Sent += (object sender, MessageArgs<ViewModelMessageContent> args) =>
            {
                UpdaterViewModel relayedViewModel = args.Content.ViewModel as UpdaterViewModel;
                relayedViewModel.DownloadUpdateCommand.Execute(null);
            };
            bool cancelTask = false;
            bool updateInstallable = false;
            vm.UpdateInstallableMessage.Sent += (object sender, MessageArgs<ViewModelMessageContent> args) => {
                UpdaterViewModel relayedViewModel = args.Content.ViewModel as UpdaterViewModel;
                updateInstallable = true;
            };
            vm.CheckForUpdateCommand.Execute(null);

            Task checkInstallableTask = new Task(() =>
            {
                while (!updateInstallable && !cancelTask) ;
            });

            checkInstallableTask.Start();
            checkInstallableTask.Wait(10000);
            // Cancel the task in case the timeout was reached but the event was not raised
            cancelTask = !updateInstallable;
            Assert.True(updateInstallable, "Update should have been downloaded and be installable, but isn't.");
        }

        /// <summary>
        /// The updater in version 0.5.0 of the Bovender assembly could through an error
        /// if the download of an update was cancelled (cf. exception id #703f87f7).
        /// The Updater class' internal _updater field may not have been set e.g. if the
        /// target file existed already and no download was necessary, resulting in a
        /// null reference exception.
        /// This test asserts that cancelling does not throw an exception.
        /// </summary>
        [Test]
        public void CancelDownloadUpdate()
        {
            UpdaterForTesting updater = new UpdaterForTesting();
            updater.TestVersion = "0.0.0";
            UpdaterViewModel vm = new UpdaterViewModel(updater);
            bool done = false;
            bool cancel = false;
            bool raised = false;

            // In order to test whether cancelling throws an exception, we need
            // to catch unhandled exceptions because there is no other way to
            // capture an exception that occurs in a different thread.
            AppDomain.CurrentDomain.UnhandledException +=
                (object sender, UnhandledExceptionEventArgs e) =>
                {
                    raised = true;
                };

            vm.UpdateAvailableMessage.Sent += (object sender, MessageArgs<ViewModelMessageContent> args) =>
            {
                UpdaterViewModel relayedViewModel = args.Content.ViewModel as UpdaterViewModel;
                relayedViewModel.DownloadUpdateMessage.Sent +=
                    (object sender2, MessageArgs<ProcessMessageContent> args2) =>
                    {
                        done = true;
                        args2.Content.CancelProcess.Invoke();
                    };
                relayedViewModel.DownloadUpdateCommand.Execute(null);
            };
            vm.CheckForUpdateCommand.Execute(null);

            Task waitForMessages = new Task(() =>
            {
                while (!done && !cancel) ;
            });

            waitForMessages.Start();
            waitForMessages.Wait(10000);
            // Cancel the task in case the timeout was reached
            // but the DownloadUpdateMessage was still not sent
            cancel = !done;
            Assert.True(done, "Did not reach the code that cancels the download...");
            Assert.False(raised, "Cancelling should not throw an exception.");
        }
    }
}
