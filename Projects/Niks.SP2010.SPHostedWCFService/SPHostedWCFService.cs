using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Activation;
using Microsoft.SharePoint;

namespace Niks.SP2010.SPHostedWCFService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "SPHostedWCFService" in both code and config file together.
    public class SPHostedWCFService : ISPHostedWCFService
    {
        public List<DocumentData> GetLists()
        {
            List<DocumentData> docData = new List<DocumentData>();
            string siteURL = "http://sp2013trialjan:37018/";
            string documentListName = "Shared Documents";
            using (SPSite spSite = new SPSite(siteURL))
            {
                SPDocumentLibrary spLibrary = (SPDocumentLibrary)spSite.RootWeb.Lists.TryGetList(documentListName);
                foreach (SPListItem listitem in spLibrary.Items)
                {
                    docData.Add(new DocumentData() { Name = listitem.Name, Title = listitem.Title });
                }
            }
            return docData;
        }
    }
}
