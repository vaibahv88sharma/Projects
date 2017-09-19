using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;

namespace SharePointCustomProjects.HVE.Files
{
    class HVETimerJob : SPJobDefinition
    {
        #region Constructor
        public HVETimerJob() : base() { }
        public HVETimerJob(string jobName, SPService service) :
            base(jobName, service, null, SPJobLockType.None)
        {
            this.Title = "Task Complete Timer";
        }
        public HVETimerJob(string jobName, SPWebApplication webapp) :
            base(jobName, webapp, null, SPJobLockType.ContentDatabase)
        {
            this.Title = "Task Complete Timer";
        }
        #endregion

        public override void Execute(Guid targetInstanceId)
        {
            SPWebApplication webApp = this.Parent as SPWebApplication;
            SPList taskList = webApp.Sites[0].RootWeb.Lists["TopViewedDocs"];
            SPListItem newTask = taskList.Items.Add();
            newTask["Title"] = "New Task" + DateTime.Now.ToString();
            newTask.Update();
        }
    }
}
