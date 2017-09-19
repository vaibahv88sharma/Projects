using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System;

namespace JSONWebService
{
    [DataContract]
    [Serializable]
    public class CustomerOrderHistory
    {
        [DataMember]
        public string ProductName { get; set; }

        [DataMember]
        public int Total { get; set; }
    }
}