using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace DeployUserControlInWebpart.CONTROLTEMPLATES
{
    public partial class UserControl1 : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Label2.Visible = false;
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            //TextBox2.Text = "User Name : " + TextBox1.Text + "Entered at : " + DateTime.Now.ToString();
            //TextBox2.Visible = true;

            Label2.Text = "User Name : " + TextBox1.Text + "Entered at : " + DateTime.Now.ToString();
            Label2.Visible = true;
        }
    }
}
