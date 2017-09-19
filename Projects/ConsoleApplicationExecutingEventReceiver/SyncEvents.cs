using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;

namespace ConsoleApplicationExecutingEventReceiver
{
    class SyncEvents : SPItemEventReceiver
    {
        public override void ItemDeleting(SPItemEventProperties properties)
        {
            CheckReferencedata("SubProgram", properties.ListItem["ProgramID"]);
            CheckReferencedata("Department", properties.ListItem["ProgramID"]);
            properties.ErrorMessage = "Parent and Child Items are deleted";
        }
        void CheckReferencedata(String Listname, Object Value)
        {
            SPSite site = new SPSite("SiteURL");
            SPWeb web = site.AllWebs["WebnameWhenthelistsarethere"];
            web.AllowUnsafeUpdates = true;
            SPList list = web.Lists[Listname];
            SPQuery Query = new SPQuery();
            Query.Query = "<Where><Eq><FieldRef Name='ProgramId'/><Value Type='Text'>" + Value + "</Value></Eq></Where>";
            SPListItemCollection AnswerItems = list.GetItems(Query);
            int Count = AnswerItems.Count;
            if (Count > 0)
            {
                for (int index = 0; index < Count; index++)
                {
                    SPListItem item = AnswerItems[index];
                    AnswerItems.Delete(index);
                }
            }
            list.Update();
        }
    }
}
