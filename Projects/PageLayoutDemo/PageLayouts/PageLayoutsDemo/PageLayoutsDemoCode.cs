using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace PageLayoutDemo.PageLayouts.PageLayoutsDemo
{
    [CLSCompliant(false)]
    public class PageLayoutsDemoCode : Microsoft.SharePoint.Publishing.PublishingLayoutPage
    {
        //protected Label Label1;
        protected Button Button1;
        protected Button Button2;
        protected Label Label1;
        protected Label Label2;
        protected TextBox TextBox1;
        protected void Page_Load(object sender, EventArgs e)
        {
            Label1.Text = "Page Load Called";
        }
        protected void GetTimeButton_Click(object sender, EventArgs e)
        {
            Label1.Text = "Now time is : " + System.DateTime.Now.ToString();
        }
        protected void SayHelloButton_Click(object sender, EventArgs e)
        {
            Label2.Text = "Hello, " + TextBox1.Text + " !!!!";
        }
    }
}
