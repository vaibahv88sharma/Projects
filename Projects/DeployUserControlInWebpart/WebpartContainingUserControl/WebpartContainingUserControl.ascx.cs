using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;

namespace DeployUserControlInWebpart.WebpartContainingUserControl
{
    [ToolboxItemAttribute(false)]
    public partial class WebpartContainingUserControl : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public WebpartContainingUserControl()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }
      //private const string _ascxPath = @"~/_CONTROLTEMPLATES/CS/VisualWebPart1/VisualWebPart1UserControl.ascx";
        private const string _ascxPath = "~/_CONTROLTEMPLATES/15/UserControl1.ascx";
                                       //"~/_controltemplates/WebPart1/SampleWebUserControl.ascx"

        protected override void CreateChildControls()
        {
            try
            {
                Control control = this.Page.LoadControl(_ascxPath);
                Controls.Add(control);
                base.CreateChildControls();
            }
            catch (Exception e) 
            {
                Console.WriteLine(e.Message);
            }
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            base.RenderContents(writer);
        }

  
    }
}
