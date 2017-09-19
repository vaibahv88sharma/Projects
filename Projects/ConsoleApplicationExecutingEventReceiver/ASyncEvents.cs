using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;

namespace ConsoleApplicationExecutingEventReceiver
{
    class ASyncEvents : SPItemEventReceiver
    {
        public override void ItemAdded(SPItemEventProperties properties)
        {
            SPListItem item = properties.ListItem;
            item["Title"] = item["Title"] + " - " + DateTime.Now;
            item.Update();
        }
    }
}
