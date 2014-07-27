using aVoicePushClient.Common;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Credentials;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace aVoicePushClient
{
    /// <summary>
    /// A Page responsible for signing the user up for push notifications.
    /// </summary>
    public sealed partial class SignInPage : Page
    {
        private readonly NavigationHelper navigationHelper;

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        public SignInPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            string navigationParameter = e.NavigationParameter as string;

            if (navigationParameter != null && navigationParameter == "signout")
            {
                // Don't let the user go back if they've signed out
                Frame.BackStack.Clear();
            }

            SwitchToProgressState();
            MessageDialog dialog = null;

            try
            {
                bool authorized = await App.IsUserAuthenticated();

                if (!authorized)
                {
                    SwitchToSignInState();
                }
                else
                {
                    // Make sure the push channel is up to date
                    await App.InitNotificationsAsync();

                    // Go to the Inbox page
                    Frame.Navigate(typeof(InboxPage));
                }
            }
            catch (MobileServiceInvalidOperationException ex)
            {
                if (ex.Response.StatusCode == HttpStatusCode.NotFound)
                {
                    // Internet might be down
                    dialog = new MessageDialog("Can't connect, please check your internet connection");
                }
                else
                {
                    dialog = new MessageDialog("Unknown error");
                }
            }

            if (dialog != null)
            {
                await App.DeleteNotificationAsync();

                dialog.Commands.Add(new UICommand("Retry", (IUICommand) =>
                {
                    NavigationHelper_LoadState(sender, e);
                }));

                await dialog.ShowAsync();
            }
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            // TODO: Save the unique state of the page here.
        }

        private async void ContinueButton_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog dialog = null;

            try
            {
                // We need the user to sign in again
                await App.AuthenticateAsync();

                // Don't let the user click sign in again
                SwitchToProgressState();

                // Register for Push notifications TODO catch exceptions and ask the user to retry (fails if Google can't get email)
                await App.InitNotificationsAsync();
            }
            catch (Exception ex)
            {
                dialog = new MessageDialog("Error: " + ex.Message);
            }

            if (dialog != null)
            {
                await dialog.ShowAsync();

                // Let them retry
                SwitchToSignInState();
            }
            else
            {
                // Navigate to the Inbox, or the tutorial if they haven't seen that yet
                bool seen = ApplicationData.Current.LocalSettings.Values["HasSeenTutorial"] as bool? ?? false;

                if (seen)
                {
                    Frame.Navigate(typeof(InboxPage));
                }
                else
                {
                    ApplicationData.Current.LocalSettings.Values["HasSeenTutorial"] = true;
                    Frame.Navigate(typeof(InboxPage), "tutorial");
                }
            }
        }

        private void SwitchToProgressState()
        {
            ContinueButton.Visibility = Visibility.Collapsed;
            ProgressPanel.Visibility = Visibility.Visible;
        }

        private void SwitchToSignInState()
        {
            ProgressPanel.Visibility = Visibility.Collapsed;
            ContinueButton.Visibility = Visibility.Visible;
        }

        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion
    }
}
