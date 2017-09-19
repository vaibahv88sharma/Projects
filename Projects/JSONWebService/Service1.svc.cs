using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.IO;
using System.Web.Script.Serialization;

namespace JSONWebService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service1 : IService1
    {
        public List<wsCustomer> GetAllCustomers()
        {
            NorthwindDataContext dc = new NorthwindDataContext();
            List<wsCustomer> results = new List<wsCustomer>();

            foreach (Customer cust in dc.Customers)
            {
                results.Add(new wsCustomer()
                {
                    CustomerID = cust.CustomerID,
                    CompanyName = cust.CompanyName,
                    City = cust.City
                });
            }

            return results;
        }

        public List<wsOrder> GetOrdersForCustomer(string customerID)
        {
            NorthwindDataContext dc = new NorthwindDataContext();
            List<wsOrder> results = new List<wsOrder>();
            System.Globalization.CultureInfo ci = System.Globalization.CultureInfo.GetCultureInfo("en-US");

            foreach (Order order in dc.Orders.Where(s => s.CustomerID == customerID))
            {
                results.Add(new wsOrder()
                {
                    OrderID = order.OrderID,
                    OrderDate = (order.OrderDate == null) ? "" : order.OrderDate.Value.ToString("d", ci),
                    ShipAddress = order.ShipAddress,
                    ShipCity = order.ShipCity,
                    ShipName = order.ShipName,
                    ShipPostcode = order.ShipPostalCode,
                    ShippedDate = (order.ShippedDate == null) ? "" : order.ShippedDate.Value.ToString("d", ci)
                });
            }

            return results;
        }

        public List<CustomerOrderHistory> GetCustomerOrderHistory(string customerID)
        {
            List<CustomerOrderHistory> results = new List<CustomerOrderHistory>();
            NorthwindDataContext dc = new NorthwindDataContext();

            foreach (CustOrderHistResult oneOrder in dc.CustOrderHist(customerID))
            {
                results.Add(new CustomerOrderHistory()
                {
                    ProductName = oneOrder.ProductName,
                    Total = oneOrder.Total ?? 0
                });
            }

            return results;
        }

        public int UpdateOrderAddress(Stream JSONdataStream)
        {
            try
            {
                // Read in our Stream into a string...
                StreamReader reader = new StreamReader(JSONdataStream);
                string JSONdata = reader.ReadToEnd();

                // ..then convert the string into a single "wsOrder" record.
                JavaScriptSerializer jss = new JavaScriptSerializer();
                wsOrder order = jss.Deserialize<wsOrder>(JSONdata);
                if (order == null)
                {
                    // Error: Couldn't deserialize our JSON string into a "wsOrder" object.
                    return -2;
                }

                NorthwindDataContext dc = new NorthwindDataContext();
                Order currentOrder = dc.Orders.Where(o => o.OrderID == order.OrderID).FirstOrDefault();
                if (currentOrder == null)
                {
                    // Couldn't find an [Order] record with this ID
                    return -3;
                }

                // Update our SQL Server [Order] record, with our new Shipping Details (send from whatever
                // app is calling this web service)
                currentOrder.ShipName = order.ShipName;
                currentOrder.ShipAddress = order.ShipAddress;
                currentOrder.ShipCity = order.ShipCity;
                currentOrder.ShipPostalCode = order.ShipPostcode;

                dc.SubmitChanges();

                return 0;     // Success !
            }
            catch (Exception)
            {
                return -1;
            }
        }
    }
}
