using aVoicePushClient.Common;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Networking.PushNotifications;
using Windows.Security.Credentials;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=234227

namespace aVoicePushClient
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : Application
    {
#if WINDOWS_PHONE_APP
        private TransitionCollection transitions;
#endif

        public static MobileServiceClient MobileService = new MobileServiceClient(
            @"https://avoicepush.azure-mobile.net/"
            );

        public static MobileServiceUser MobileUser;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += this.OnSuspending;
            this.Resuming += OnResuming;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected async override void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                //Associate the frame with a SuspensionManager key                                
                SuspensionManager.RegisterFrame(rootFrame, "AppFrame");

                // TODO: change this value to a cache size that is appropriate for your application
                rootFrame.CacheSize = 1;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // Restore the saved session state only when appropriate
                    try
                    {
                        await SuspensionManager.RestoreAsync();
                    }
                    catch (SuspensionManagerException)
                    {
                        // Something went wrong restoring state.
                        // Assume there is no state and continue
                    }
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
#if WINDOWS_PHONE_APP
                // Removes the turnstile navigation for startup.
                if (rootFrame.ContentTransitions != null)
                {
                    this.transitions = new TransitionCollection();
                    foreach (var c in rootFrame.ContentTransitions)
                    {
                        this.transitions.Add(c);
                    }
                }

                rootFrame.ContentTransitions = null;
                rootFrame.Navigated += this.RootFrame_FirstNavigated;
#endif

                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                if (!rootFrame.Navigate(typeof(SignInPage), e.Arguments))
                {
                    throw new Exception("Failed to create initial page");
                }
            }

            // Ensure the current window is active
            Window.Current.Activate();
        }

        protected override void OnActivated(IActivatedEventArgs args)
        {
            base.OnActivated(args);

#if WINDOWS_PHONE_APP
            if (args.Kind == ActivationKind.WebAuthenticationBrokerContinuation)
            {
                MobileService.LoginComplete(args as WebAuthenticationBrokerContinuationEventArgs);
            }
#endif
        }

#if WINDOWS_PHONE_APP
        /// <summary>
        /// Restores the content transitions after the app has launched.
        /// </summary>
        /// <param name="sender">The object where the handler is attached.</param>
        /// <param name="e">Details about the navigation event.</param>
        private void RootFrame_FirstNavigated(object sender, NavigationEventArgs e)
        {
            var rootFrame = sender as Frame;
            rootFrame.ContentTransitions = this.transitions ?? new TransitionCollection() { new NavigationThemeTransition() };
            rootFrame.Navigated -= this.RootFrame_FirstNavigated;
        }
#endif

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            await SuspensionManager.SaveAsync();

            deferral.Complete();
        }

        private void OnResuming(object sender, object e)
        {
            
        }

        /// <summary>
        /// Attempt to send the calls to deregister the user. Will silently fail if there are
        /// networking issues.
        /// </summary>
        /// <returns></returns>
        public static async Task TryDeleteNotificationAsync()
        {
            try
            {
                await MobileService.GetPush().UnregisterNativeAsync();

                // Mobile Services doesn't have an API injection point for unregistering a push (just registering)
                await MobileService.InvokeApiAsync("unregister", HttpMethod.Get, null);
            }
            catch (MobileServiceInvalidOperationException)
            {
                // We'll see the errors on the server
            }

            MobileService.Logout();

            PasswordVault vault = new PasswordVault();
            
            foreach (PasswordCredential cred in vault.RetrieveAll())
            {
                vault.Remove(cred);
            }
        }

        public static async Task InitNotificationsAsync()
        {
            // Request a push notification channel.
            var channel = await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();

            // Register for notifications using the new channel
            await MobileService.GetPush().RegisterNativeAsync(channel.Uri);
        }

        /// <summary>
        /// Determine if the user has entered their credentials, and if they have, whether
        /// those are still valid or not.
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> IsUserAuthenticated()
        {
            PasswordCredential cred = PopulateMobileUserFromCache();
            return await IsMobileUserAuthorizedAsync(cred);
        }

        /// <summary>
        /// Check the PasswordVault for MobileServiceUserCredentials and populate App.MobileUser.
        /// </summary>
        /// <returns>The saved PasswordCredential, or null if not found</returns>
        private static PasswordCredential PopulateMobileUserFromCache()
        {
            PasswordVault vault = new PasswordVault();
            PasswordCredential credential = null;

            try
            {
                // Try to get an existing credential from the vault.
                credential = vault.FindAllByResource("Google").FirstOrDefault();
            }
            catch (Exception)
            {
                // When there is no matching resource an error occurs, which we ignore.
            }

            if (credential != null)
            {
                // Create a user from the stored credentials.
                MobileUser = new MobileServiceUser(credential.UserName);
                credential.RetrievePassword();
                MobileUser.MobileServiceAuthenticationToken = credential.Password;

                // Set the user from the stored credentials.
                App.MobileService.CurrentUser = MobileUser;
            }
            
            return credential;
        }

        /// <summary>
        /// Test whether a simple service call works.
        /// </summary>
        /// <param name="credential">The saved credential for the MobileUser</param>
        /// <returns>True if authorized</returns>
        private static async Task<bool> IsMobileUserAuthorizedAsync(PasswordCredential credential)
        {
            PasswordVault vault = new PasswordVault();

            try
            {
                // Try to return an item now to determine if the cached credential has expired.
                await App.MobileService.InvokeApiAsync("checkauth", HttpMethod.Get, null);
            }
            catch (MobileServiceInvalidOperationException ex)
            {
                if (ex.Response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    // Remove the credential with the expired token.
                    if (credential != null)
                        vault.Remove(credential);

                    return false;
                }

                throw;
            }

            return true;
        }

        /// <summary>
        /// Log into the Mobile Service and cache the user data.
        /// </summary>
        /// <param name="vault"></param>
        /// <returns></returns>
        /// <exception cref="MobileServiceInvalidOperationException">Server returned failure</exception>
        public static async Task AuthenticateAsync()
        {
            PasswordVault vault = new PasswordVault();

            // Login with the identity provider.
            MobileUser = await App.MobileService.LoginAsync(MobileServiceAuthenticationProvider.Google);

            // Create and store the user credentials.
            PasswordCredential credential = new PasswordCredential("Google", MobileUser.UserId, MobileUser.MobileServiceAuthenticationToken);
            vault.Add(credential);
        }
    }
}