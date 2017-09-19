using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Niks.SP2010.SPHostedWCFService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ISPHostedWCFService" in both code and config file together.
    [ServiceContract]
    public interface ISPHostedWCFService
    {
        [OperationContract]
        List<DocumentData> GetLists();
    }

    [DataContract]
    public class DocumentData
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Title { get; set; }
    }
}
