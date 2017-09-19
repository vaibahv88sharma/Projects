using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;

namespace SharePointProj666.Features.Feature1
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("ff827489-4ce2-4912-aff0-7ed58f19c634")]
    public class Feature1EventReceiver : SPFeatureReceiver
    {
        // Uncomment the method below to handle the event raised after a feature has been activated.


        const string JobName = "HVETimerJob1234";
        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    SPWebApplication parentWebApp = (SPWebApplication)properties.Feature.Parent;
                    SPSite site = properties.Feature.Parent as SPSite;
                    DeleteExistingJob(JobName, parentWebApp);
                    CreateJob(parentWebApp);
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region TimerJob Methods
        private bool CreateJob(SPWebApplication site)
        {
            bool jobCreated = false;
            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    CustCode job = new CustCode(JobName, site);
                    SPDailySchedule schedule = new SPDailySchedule();
                    schedule.BeginHour = 1;
                    schedule.EndHour = 1;
                    job.Schedule = schedule;
                    job.Update();

                });

            }
            catch (Exception)
            {
                return jobCreated;
            }
            return jobCreated;
        }

        public bool DeleteExistingJob(string jobName, SPWebApplication site)
        {
            bool jobDeleted = false;
            try
            {
                foreach (SPJobDefinition job in site.JobDefinitions)
                {
                    if (job.Name == jobName)
                    {
                        job.Delete();
                        jobDeleted = true;
                    }
                }
            }
            catch (Exception)
            {
                return jobDeleted;
            }
            return jobDeleted;
        }
        #endregion


        // Uncomment the method below to handle the event raised before a feature is deactivated.

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            lock (this)
            {
                try
                {
                    SPSecurity.RunWithElevatedPrivileges(delegate()
                    {
                        SPWebApplication parentWebApp = (SPWebApplication)properties.Feature.Parent;
                        // //DeleteExistingTimerJobFromSite(this.JobName, parentWebApp);
                        DeleteExistingJob(JobName, parentWebApp);



                        // SPSite oSite = properties.Feature.Parent as SPSite;
                        // SPWebApplication webApp = oSite.WebApplication;
                        // DeleteExistingJob(JobName, webApp);
                        // CreateJob(webApp);

                    });
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }


        // Uncomment the method below to handle the event raised after a feature has been installed.

        //public override void FeatureInstalled(SPFeatureReceiverProperties properties)
        //{
        //}


        // Uncomment the method below to handle the event raised before a feature is uninstalled.

        //public override void FeatureUninstalling(SPFeatureReceiverProperties properties)
        //{
        //}

        // Uncomment the method below to handle the event raised when a feature is upgrading.

        //public override void FeatureUpgrading(SPFeatureReceiverProperties properties, string upgradeActionName, System.Collections.Generic.IDictionary<string, string> parameters)
        //{
        //}
    }
}
