using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.IO;

namespace JSONWebService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IService1
    {
        [OperationContract]
        [WebInvoke(Method = "GET", 
                    ResponseFormat = WebMessageFormat.Json, 
                    BodyStyle = WebMessageBodyStyle.Wrapped, 
                    UriTemplate = "getAllCustomers")]
        List<wsCustomer> GetAllCustomers();

        [OperationContract]
        [WebInvoke(Method = "GET", 
                    ResponseFormat = WebMessageFormat.Json, 
                    BodyStyle = WebMessageBodyStyle.Wrapped, 
                    UriTemplate = "getOrdersForCustomer/{customerID}")]
        List<wsOrder> GetOrdersForCustomer(string customerID);

        [OperationContract]
        [WebInvoke(Method = "GET", 
                    ResponseFormat = WebMessageFormat.Json, 
                    UriTemplate = "getCustomerOrderHistory/{customerID}")]
        List<CustomerOrderHistory> GetCustomerOrderHistory(string customerID);

        [OperationContract]
        [WebInvoke(Method = "POST", 
                    ResponseFormat = WebMessageFormat.Json, 
                    UriTemplate = "updateOrderAddress")]
        int UpdateOrderAddress(Stream JSONdataStream);
    }
}
