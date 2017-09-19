using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;

namespace ConsoleApplicationExecutingEventReceiver
{
    class Program
    {
        static void Main(string[] args)
        {
            #region ItemAdded
            try
            {
                using (SPSite site = new SPSite("http://sp:1220/sites/SPSite/"))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        SPList list = web.Lists["Country"];

                        SPEventReceiverDefinition def = list.EventReceivers.Add();

                        def.Assembly = "ConsoleApplicationExecutingEventReceiver, Version=1.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c";//604f58e28557db00
                        def.Class = "ConsoleApplicationExecutingEventReceiver.ASyncEvents";
                        def.Name = "ItemAdded Event";
                        def.Type = SPEventReceiverType.ItemAdded;
                        def.SequenceNumber = 1000;
                        def.Synchronization = SPEventReceiverSynchronization.Synchronous;
                        def.Update();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            #endregion

            #region ItemDeleting

          //  Console.ReadKey();
          //  SPSite collection = new SPSite(http://sitename/);
          //  SPUser user = new SPUser();
          //  SPWeb site = collection.RootWeb;
          //  SPList list = site.Lists["Program"];
          //  string asmName = "DeleteData, Version=1.0.0.0, Culture=neutral, PublicKeyToken=21ce19119994750e";
          //  string className = " DeleteData.DeleteRefrence";
          //  // Register the events with the list
          //  list.EventReceivers.Add(SPEventReceiverType.ItemDeleting, asmName, className);
          //// Clean up the code
          //  site.Dispose();
          //  collection.Dispose();
          //  Console.WriteLine("Sucessfully Registered");
          //  // Return to calling environment : Success
          //  Environment.Exit(0);

            #endregion
        }
    }
}
