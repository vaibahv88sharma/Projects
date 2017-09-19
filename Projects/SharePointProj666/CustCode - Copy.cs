using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;

namespace SharePointProj666
{
    class CustCode : SPJobDefinition
    {
        public CustCode() : base() {  }
        
        public CustCode(string jobName, SPService service) :
            base(jobName, service, null, SPJobLockType.None)
        {
            this.Title = jobName;
            //this.Title = "Task Complete Timer1";
        }

        public CustCode(string jobName, SPWebApplication webapp) :
            base(jobName, webapp, null, SPJobLockType.ContentDatabase)
        {
            this.Title = jobName;
        }

        public override void Execute(Guid targetInstanceId)
        {
            ////SPWebApplication webapp = this.Parent as SPWebApplication;
            ////SPSite site1 = webapp.Sites["http://win-njfp7te48bn/sites/HVEDev"];
            SPSite site1 = new SPSite("http://win-njfp7te48bn/sites/HVEDev");
            SPWeb web1 = site1.OpenWeb();
            SPList tasklist = web1.Lists["Tasks1"];
            SPListItem li = tasklist.Items.Add();
            li["Title"] = "New Task :- " + DateTime.Now.ToString();
            li.Update();

            //////webapp.Sites.Where(p => p.ServerRelativeUrl.Equals)
            ////SPWebApplication webapp = this.Parent as SPWebApplication;           
            ////SPList tasklist = webapp.Sites[0].RootWeb.Lists["Tasks1"];
            ////SPListItem newTask = tasklist.Items.Add();
            ////newTask["Title"] = "New Task :- " + DateTime.Now.ToString();
            ////newTask.Update();


           
        }
    }
}
