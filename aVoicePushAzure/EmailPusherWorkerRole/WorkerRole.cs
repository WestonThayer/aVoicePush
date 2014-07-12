using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using System.Collections.ObjectModel;
using Services;

namespace EmailPusher
{
    public class WorkerRole : RoleEntryPoint
    {
        // This is a list of configuration settings (in ServiceConfiguration.[Config].cscfg) that if changed
        // should not result in the role being recycled. This is implemented in the RoleEnvironmentChanging
        // and RoleEnvironmentChanged callbacks below.
        private static string[] exemptConfigurationItems = new[]
        {
            "SqlConnectionString",
            "ClientId",
            "ClientSecret"
        };

        /// <summary>
        /// A handle to our SMTP server. This will take care of all the internals of
        /// parsing an email from GV and sending a push notification.
        /// </summary>
        private SmtpHandler smtpHandler;

        public override void Run()
        {
            ServiceLocator.Current.Log.Information("Run() entry");

            smtpHandler.Run();

            ServiceLocator.Current.Log.Warning("Run() exit for unknown reason!");
        }

        public override bool OnStart()
        {
            ServiceLocator.Current.Log = new Tr();

            ServiceLocator.Current.Log.Information("OnStart() entry. Processor count is: " + Environment.ProcessorCount);

            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = Environment.ProcessorCount;

            DiagnosticMonitor.Start("Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString");

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.
            RoleEnvironment.Changing += RoleEnvironment_Changing;
            RoleEnvironment.Changed += RoleEnvironment_Changed;

            smtpHandler = new SmtpHandler();

            return base.OnStart();
        }

        public override void OnStop()
        {
            ServiceLocator.Current.Log.Information("OnStop() entry");

            base.OnStop();
        }

        /// <summary>
        /// RoleEventChanging - Called when a change is about to be applied to the role.  Determines whether or not to recycle the role instance.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">A list of what is changing</param>
        private void RoleEnvironment_Changing(object sender, RoleEnvironmentChangingEventArgs e)
        {
            // Note: e.Cancel == true -> Azure should recycle the role.  If all the changes are in our "exempt" list,
            // we don't need to recycle the role.
            e.Cancel = HasNonExemptConfigurationChanges(e.Changes);

            if (!e.Cancel)
                ServiceLocator.Current.Log.Information("WorkerRole::RoleEnvironmentChanging - role is not recycling.");
            else
                ServiceLocator.Current.Log.Information("WorkerRole::RoleEnvironmentChanging - recycling role instance.");
        }

        /// <summary>
        /// RoleEnvironmentChanged - Called after a change has been applied to the role.  
        /// NOTE: This is called AFTER RoleEnvironmentChanging is called.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">List of what has changed</param>
        private void RoleEnvironment_Changed(object sender, RoleEnvironmentChangedEventArgs e)
        {

        }

        /// <summary>
        /// HasNonExemptConfigurationChanges - Check if config changes contain any that aren't on our "exempt from recycle" list
        /// Returns TRUE if there is at least one config change that isn't on our list.
        /// </summary>
        /// <param name="chgs">Collection of changes from RoleEnvironmentChanging or RoleEnvironmentChanged</param>
        /// <returns></returns>
        private bool HasNonExemptConfigurationChanges(ReadOnlyCollection<RoleEnvironmentChange> chgs)
        {
            Func<RoleEnvironmentConfigurationSettingChange, bool> changeIsNonExempt =
                    x => !exemptConfigurationItems.Contains(x.ConfigurationSettingName);

            var environmentChanges = chgs.OfType<RoleEnvironmentConfigurationSettingChange>();

            return environmentChanges.Any(changeIsNonExempt);
        }
    }
}
